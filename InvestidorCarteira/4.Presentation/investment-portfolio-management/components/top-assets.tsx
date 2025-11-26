"use client"

import type { Asset } from "@/context/portfolio-context"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { TrendingUp, TrendingDown } from "lucide-react"

interface TopAssetsProps {
  assets: Asset[]
}

export function TopAssets({ assets }: TopAssetsProps) {
  const sortedAssets = [...assets]
    .map((asset) => ({
      ...asset,
      totalValue: asset.quantity * asset.currentPrice,
      totalCost: asset.quantity * asset.purchasePrice,
      gain: asset.quantity * asset.currentPrice - asset.quantity * asset.purchasePrice,
      gainPercent: ((asset.currentPrice - asset.purchasePrice) / asset.purchasePrice) * 100,
    }))
    .sort((a, b) => b.totalValue - a.totalValue)

  return (
    <Card>
      <CardHeader>
        <CardTitle>Seus Ativos</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b">
                <th className="text-left py-2 font-medium">Ativo</th>
                <th className="text-left py-2 font-medium">Qtd</th>
                <th className="text-right py-2 font-medium">Preço</th>
                <th className="text-right py-2 font-medium">Valor Total</th>
                <th className="text-right py-2 font-medium">Ganho</th>
                <th className="text-right py-2 font-medium">Rentabilidade</th>
              </tr>
            </thead>
            <tbody>
              {sortedAssets.map((asset) => (
                <tr key={asset.id} className="border-b hover:bg-secondary/50">
                  <td className="py-3">
                    <div>
                      <p className="font-medium">{asset.symbol}</p>
                      <p className="text-xs text-muted-foreground">{asset.name}</p>
                    </div>
                  </td>
                  <td className="py-3">{asset.quantity}</td>
                  <td className="text-right py-3">R$ {asset.currentPrice.toFixed(2)}</td>
                  <td className="text-right py-3 font-medium">R$ {asset.totalValue.toFixed(2)}</td>
                  <td className={`text-right py-3 font-medium ${asset.gain >= 0 ? "text-green-600" : "text-red-600"}`}>
                    R$ {asset.gain.toFixed(2)}
                  </td>
                  <td className="text-right py-3">
                    <div
                      className={`flex items-center justify-end gap-1 ${asset.gainPercent >= 0 ? "text-green-600" : "text-red-600"}`}
                    >
                      {asset.gainPercent >= 0 ? (
                        <TrendingUp className="w-4 h-4" />
                      ) : (
                        <TrendingDown className="w-4 h-4" />
                      )}
                      <span>{asset.gainPercent.toFixed(2)}%</span>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>
  )
}
