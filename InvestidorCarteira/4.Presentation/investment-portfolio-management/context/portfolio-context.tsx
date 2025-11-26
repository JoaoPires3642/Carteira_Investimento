"use client"

import type React from "react"
import { createContext, useContext, useState, useEffect } from "react"

export interface Asset {
  id: string
  name: string
  symbol: string
  type: "stock" | "etf" | "crypto" | "fund" | "fixed-income" | "real-estate"
  quantity: number
  purchasePrice: number
  currentPrice: number
  purchaseDate: string
  sector?: string
  notes?: string
}

export interface Portfolio {
  assets: Asset[]
  totalInvested: number
  currentValue: number
  totalGain: number
  totalGainPercent: number
  currency: string
}

interface PortfolioContextType {
  portfolio: Portfolio
  addAsset: (asset: Omit<Asset, "id">) => void
  updateAsset: (id: string, asset: Partial<Asset>) => void
  deleteAsset: (id: string) => void
  updatePrice: (id: string, price: number) => void
}

const PortfolioContext = createContext<PortfolioContextType | undefined>(undefined)

export function PortfolioProvider({ children }: { children: React.ReactNode }) {
  const [portfolio, setPortfolio] = useState<Portfolio>({
    assets: [],
    totalInvested: 0,
    currentValue: 0,
    totalGain: 0,
    totalGainPercent: 0,
    currency: "BRL",
  })

  // Load from localStorage
  useEffect(() => {
    const saved = localStorage.getItem("portfolio")
    if (saved) {
      try {
        setPortfolio(JSON.parse(saved))
      } catch (error) {
        console.error("Error loading portfolio:", error)
      }
    }
  }, [])

  // Save to localStorage
  useEffect(() => {
    localStorage.setItem("portfolio", JSON.stringify(portfolio))
  }, [portfolio])

  // Calculate totals
  useEffect(() => {
    const totalInvested = portfolio.assets.reduce((sum, asset) => sum + asset.quantity * asset.purchasePrice, 0)
    const currentValue = portfolio.assets.reduce((sum, asset) => sum + asset.quantity * asset.currentPrice, 0)
    const totalGain = currentValue - totalInvested
    const totalGainPercent = totalInvested > 0 ? (totalGain / totalInvested) * 100 : 0

    setPortfolio((prev) => ({
      ...prev,
      totalInvested,
      currentValue,
      totalGain,
      totalGainPercent,
    }))
  }, [portfolio.assets])

  const addAsset = (asset: Omit<Asset, "id">) => {
    const newAsset: Asset = {
      ...asset,
      id: Date.now().toString(),
    }
    setPortfolio((prev) => ({
      ...prev,
      assets: [...prev.assets, newAsset],
    }))
  }

  const updateAsset = (id: string, updates: Partial<Asset>) => {
    setPortfolio((prev) => ({
      ...prev,
      assets: prev.assets.map((asset) => (asset.id === id ? { ...asset, ...updates } : asset)),
    }))
  }

  const deleteAsset = (id: string) => {
    setPortfolio((prev) => ({
      ...prev,
      assets: prev.assets.filter((asset) => asset.id !== id),
    }))
  }

  const updatePrice = (id: string, price: number) => {
    updateAsset(id, { currentPrice: price })
  }

  return (
    <PortfolioContext.Provider
      value={{
        portfolio,
        addAsset,
        updateAsset,
        deleteAsset,
        updatePrice,
      }}
    >
      {children}
    </PortfolioContext.Provider>
  )
}

export function usePortfolio() {
  const context = useContext(PortfolioContext)
  if (!context) {
    throw new Error("usePortfolio must be used within PortfolioProvider")
  }
  return context
}
