"use client"

import type { Asset } from "@/context/portfolio-context"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

interface AssetAllocationProps {
  assets: Asset[]
}

const TYPE_COLORS: Record<string, string> = {
  stock: "#3b82f6",
  etf: "#8b5cf6",
  crypto: "#f59e0b",
  fund: "#10b981",
  "fixed-income": "#06b6d4",
  "real-estate": "#ef4444",
}

const TYPE_LABELS: Record<string, string> = {
  stock: "Ações",
  etf: "ETFs",
  crypto: "Criptomoedas",
  fund: "Fundos",
  "fixed-income": "Renda Fixa",
  "real-estate": "Imóveis",
}

export function AssetAllocation({ assets }: AssetAllocationProps) {
  const allocation = assets.reduce(
    (acc, asset) => {
      const value = asset.quantity * asset.currentPrice
      acc[asset.type] = (acc[asset.type] || 0) + value
      return acc
    },
    {} as Record<string, number>,
  )

  const total = Object.values(allocation).reduce((sum, val) => sum + val, 0)
  const allocations = Object.entries(allocation).map(([type, value]) => ({
    type,
    value,
    percent: total > 0 ? (value / total) * 100 : 0,
  }))

  return (
    <Card>
      <CardHeader>
        <CardTitle>Alocação de Ativos</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          {allocations.map(({ type, value, percent }) => (
            <div key={type}>
              <div className="flex items-center justify-between mb-1">
                <span className="text-sm font-medium">{TYPE_LABELS[type]}</span>
                <span className="text-sm text-muted-foreground">{percent.toFixed(1)}%</span>
              </div>
              <div className="w-full bg-secondary rounded-full h-2">
                <div
                  className="h-2 rounded-full"
                  style={{
                    width: `${percent}%`,
                    backgroundColor: TYPE_COLORS[type],
                  }}
                />
              </div>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}
