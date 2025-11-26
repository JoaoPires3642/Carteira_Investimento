"use client"

import type { Asset } from "@/context/portfolio-context"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Trash2, Edit2, TrendingUp, TrendingDown } from "lucide-react"

interface AssetListProps {
  assets: Asset[]
  onEdit: (id: string) => void
  onDelete: (id: string) => void
}

const TYPE_LABELS: Record<string, string> = {
  stock: "Ação",
  etf: "ETF",
  crypto: "Criptomoeda",
  fund: "Fundo",
  "fixed-income": "Renda Fixa",
  "real-estate": "Imóvel",
}

export function AssetList({ assets, onEdit, onDelete }: AssetListProps) {
  if (assets.length === 0) {
    return (
      <Card>
        <CardContent className="pt-6">
          <p className="text-center text-muted-foreground py-8">Nenhum ativo adicionado ainda</p>
        </CardContent>
      </Card>
    )
  }

  return (
    <div className="grid gap-4">
      {assets.map((asset) => {
        const totalValue = asset.quantity * asset.currentPrice
        const totalCost = asset.quantity * asset.purchasePrice
        const gain = totalValue - totalCost
        const gainPercent = totalCost > 0 ? (gain / totalCost) * 100 : 0

        return (
          <Card key={asset.id} className="hover:shadow-md transition-shadow">
            <CardContent className="pt-6">
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
                <div>
                  <p className="text-xs text-muted-foreground uppercase tracking-wide">Ativo</p>
                  <p className="text-lg font-bold">{asset.symbol}</p>
                  <p className="text-sm text-muted-foreground">{asset.name}</p>
                  <p className="text-xs text-muted-foreground mt-1">{TYPE_LABELS[asset.type]}</p>
                </div>

                <div>
                  <p className="text-xs text-muted-foreground uppercase tracking-wide">Posição</p>
                  <p className="text-lg font-bold">{asset.quantity}</p>
                  <p className="text-sm text-muted-foreground">R$ {asset.currentPrice.toFixed(2)}</p>
                </div>

                <div>
                  <p className="text-xs text-muted-foreground uppercase tracking-wide">Valor Total</p>
                  <p className="text-lg font-bold">R$ {totalValue.toFixed(2)}</p>
                  <p className="text-sm text-muted-foreground">Custo: R$ {totalCost.toFixed(2)}</p>
                </div>

                <div>
                  <p className="text-xs text-muted-foreground uppercase tracking-wide">Performance</p>
                  <p
                    className={`text-lg font-bold flex items-center gap-1 ${gain >= 0 ? "text-green-600" : "text-red-600"}`}
                  >
                    {gain >= 0 ? <TrendingUp className="w-4 h-4" /> : <TrendingDown className="w-4 h-4" />}
                    R$ {gain.toFixed(2)}
                  </p>
                  <p className={`text-sm font-medium ${gainPercent >= 0 ? "text-green-600" : "text-red-600"}`}>
                    {gainPercent.toFixed(2)}%
                  </p>
                </div>
              </div>

              {asset.notes && (
                <div className="border-t pt-3 mb-4">
                  <p className="text-xs text-muted-foreground mb-1">Notas</p>
                  <p className="text-sm">{asset.notes}</p>
                </div>
              )}

              <div className="flex gap-2 justify-end border-t pt-4">
                <Button size="sm" variant="outline" onClick={() => onEdit(asset.id)} className="gap-2">
                  <Edit2 className="w-4 h-4" />
                  Editar
                </Button>
                <Button
                  size="sm"
                  variant="destructive"
                  onClick={() => {
                    if (confirm("Tem certeza que deseja deletar este ativo?")) {
                      onDelete(asset.id)
                    }
                  }}
                  className="gap-2"
                >
                  <Trash2 className="w-4 h-4" />
                  Deletar
                </Button>
              </div>
            </CardContent>
          </Card>
        )
      })}
    </div>
  )
}
