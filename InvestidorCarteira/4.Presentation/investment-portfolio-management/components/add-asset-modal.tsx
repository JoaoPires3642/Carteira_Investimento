"use client"

import type React from "react"

import { useState } from "react"
import { usePortfolio } from "@/context/portfolio-context"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { X } from "lucide-react"

interface AddAssetModalProps {
  isOpen: boolean
  onClose: () => void
}

export function AddAssetModal({ isOpen, onClose }: AddAssetModalProps) {
  const { portfolio, addAsset, updateAsset, deleteAsset } = usePortfolio()
  const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000"
  const [formData, setFormData] = useState({
    mode: "buy" as "buy" | "sell",
    name: "",
    symbol: "",
    type: "stock" as const,
    quantity: "",
    price: "",
    purchaseDate: new Date().toISOString().split("T")[0],
  })

  async function resolvePortfolioId(): Promise<string | null> {
    const fromEnv = process.env.NEXT_PUBLIC_PORTFOLIO_ID || null
    const fromStorage = typeof window !== "undefined" ? localStorage.getItem("portfolioId") : null
    const id = fromEnv || fromStorage
    if (id) return id
    const res = await fetch(`${API_URL}/api/Portfolio`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ nomeTitular: "Frontend" })
    })
    if (!res.ok) return null
    const data = await res.json()
    if (typeof window !== "undefined") localStorage.setItem("portfolioId", data.id)
    return data.id
  }

  function mapTipoAtivo(type: string): string {
    switch (type) {
      case "stock":
        return "Acoes"
      case "etf":
        return "ETFs"
      case "crypto":
        return "Criptomoedas"
      case "fund":
        return "FIIs"
      case "real-estate":
        return "FIIs"
      default:
        return ""
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!(formData.symbol && formData.quantity && formData.price)) {
      return
    }
    const portfolioId = await resolvePortfolioId()
    if (!portfolioId) {
      alert("Não foi possível obter o ID da carteira")
      return
    }
    if (formData.mode === "buy") {
      const tipo = mapTipoAtivo(formData.type)
      if (!tipo) {
        alert("Tipo de ativo não suportado pela API")
        return
      }
      const res = await fetch(`${API_URL}/api/Portfolio/${portfolioId}/comprar`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          ticker: formData.symbol,
          quantidade: Number.parseFloat(formData.quantity),
          precoPago: Number.parseFloat(formData.price),
          tipo
        })
      })
      if (!res.ok) {
        const text = await res.text()
        alert(text || "Erro ao registrar compra")
        return
      }
      addAsset({
        name: formData.name || formData.symbol,
        symbol: formData.symbol,
        type: formData.type,
        quantity: Number.parseFloat(formData.quantity),
        purchasePrice: Number.parseFloat(formData.price),
        currentPrice: Number.parseFloat(formData.price),
        purchaseDate: formData.purchaseDate,
      })
    } else {
      const res = await fetch(`${API_URL}/api/Portfolio/${portfolioId}/vender`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          ticker: formData.symbol,
          quantidade: Number.parseFloat(formData.quantity),
          precoVenda: Number.parseFloat(formData.price)
        })
      })
      if (!res.ok) {
        const text = await res.text()
        alert(text || "Erro ao registrar venda")
        return
      }
      const existing = portfolio.assets.find(a => a.symbol.toUpperCase() === formData.symbol.toUpperCase())
      if (!existing) {
        alert("Ativo não encontrado na carteira local")
      } else {
        const remaining = existing.quantity - Number.parseFloat(formData.quantity)
        if (remaining <= 0) {
          deleteAsset(existing.id)
        } else {
          updateAsset(existing.id, { quantity: remaining })
        }
      }
    }
    setFormData({
      mode: "buy",
      name: "",
      symbol: "",
      type: "stock",
      quantity: "",
      price: "",
      purchaseDate: new Date().toISOString().split("T")[0],
    })
    onClose()
  }

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <Card className="w-full max-w-md">
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-4">
          <CardTitle>Movimentar Ativo</CardTitle>
          <button onClick={onClose} className="text-muted-foreground hover:text-foreground transition-colors">
            <X className="w-5 h-5" />
          </button>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="flex gap-2">
              <Button type="button" variant={formData.mode === "buy" ? "default" : "outline"} onClick={() => setFormData({ ...formData, mode: "buy" })} className="flex-1">Comprar</Button>
              <Button type="button" variant={formData.mode === "sell" ? "default" : "outline"} onClick={() => setFormData({ ...formData, mode: "sell" })} className="flex-1">Vender</Button>
            </div>
            <div>
              <label className="text-sm font-medium">Nome do Ativo</label>
              <input
                type="text"
                placeholder="Ex: Apple Inc"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                className="w-full px-3 py-2 border border-border rounded-md bg-background mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
                required={formData.mode === "buy"}
                readOnly={formData.mode === "sell"}
              />
            </div>

            <div>
              <label className="text-sm font-medium">Símbolo</label>
              {formData.mode === "buy" ? (
                <input
                  type="text"
                  placeholder="Ex: AAPL"
                  value={formData.symbol}
                  onChange={(e) => setFormData({ ...formData, symbol: e.target.value })}
                  className="w-full px-3 py-2 border border-border rounded-md bg-background mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
                  required
                />
              ) : (
                <select
                  value={formData.symbol}
                  onChange={(e) => {
                    const sym = e.target.value
                    const asset = portfolio.assets.find(a => a.symbol === sym)
                    setFormData({
                      ...formData,
                      symbol: sym,
                      name: asset ? asset.name : "",
                      type: asset ? (asset.type as any) : formData.type,
                    })
                  }}
                  className="w-full px-3 py-2 border border-border rounded-md bg-background mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
                  required
                >
                  <option value="" disabled>Selecione um ativo</option>
                  {portfolio.assets.map(a => (
                    <option key={a.id} value={a.symbol}>{a.symbol}</option>
                  ))}
                </select>
              )}
            </div>

            <div>
              <label className="text-sm font-medium">Tipo</label>
              {formData.mode === "buy" ? (
                <select
                  value={formData.type}
                  onChange={(e) => setFormData({ ...formData, type: e.target.value as any })}
                  className="w-full px-3 py-2 border border-border rounded-md bg-background mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
                >
                  <option value="stock">Ações</option>
                  <option value="etf">ETFs</option>
                  <option value="crypto">Criptomoedas</option>
                  <option value="fund">Fundos</option>
                  <option value="fixed-income">Renda Fixa</option>
                  <option value="real-estate">Imóveis</option>
                </select>
              ) : (
                <input
                  type="text"
                  value={formData.type}
                  readOnly
                  className="w-full px-3 py-2 border border-border rounded-md bg-background mt-1"
                />
              )}
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium">Quantidade</label>
                <input
                  type="number"
                  step="0.01"
                  placeholder="0.00"
                  value={formData.quantity}
                  onChange={(e) => setFormData({ ...formData, quantity: e.target.value })}
                  className="w-full px-3 py-2 border border-border rounded-md bg-background mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
                  required
                />
              </div>

              <div>
                <label className="text-sm font-medium">{formData.mode === "buy" ? "Preço Compra" : "Preço Venda"}</label>
                <input
                  type="number"
                  step="0.01"
                  placeholder="0.00"
                  value={formData.price}
                  onChange={(e) => setFormData({ ...formData, price: e.target.value })}
                  className="w-full px-3 py-2 border border-border rounded-md bg-background mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
                  required
                />
              </div>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium">{formData.mode === "buy" ? "Data Compra" : "Data Venda"}</label>
                <input
                  type="date"
                  value={formData.purchaseDate}
                  onChange={(e) => setFormData({ ...formData, purchaseDate: e.target.value })}
                  className="w-full px-3 py-2 border border-border rounded-md bg-background mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
                  required
                />
              </div>
            </div>

            <div className="flex gap-2 pt-4">
              <Button type="button" variant="outline" onClick={onClose} className="flex-1 bg-transparent">
                Cancelar
              </Button>
              <Button type="submit" className="flex-1">
                Adicionar
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
