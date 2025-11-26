"use client"

import type { Portfolio } from "@/context/portfolio-context"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

interface PerformanceMetricsProps {
  portfolio: Portfolio
}

export function PerformanceMetrics({ portfolio }: PerformanceMetricsProps) {
  const calculateMetrics = () => {
    if (portfolio.assets.length === 0) return null

    const withGain = portfolio.assets.filter((a) => a.quantity * a.currentPrice - a.quantity * a.purchasePrice > 0)
    const withLoss = portfolio.assets.filter((a) => a.quantity * a.currentPrice - a.quantity * a.purchasePrice < 0)

    return {
      winRate: ((withGain.length / portfolio.assets.length) * 100).toFixed(1),
      topGainer: portfolio.assets.reduce((prev, current) => {
        const prevGain =
          ((prev.quantity * prev.currentPrice - prev.quantity * prev.purchasePrice) /
            (prev.quantity * prev.purchasePrice)) *
          100
        const currentGain =
          ((current.quantity * current.currentPrice - current.quantity * current.purchasePrice) /
            (current.quantity * current.purchasePrice)) *
          100
        return currentGain > prevGain ? current : prev
      }),
      topLoser: portfolio.assets.reduce((prev, current) => {
        const prevGain =
          ((prev.quantity * prev.currentPrice - prev.quantity * prev.purchasePrice) /
            (prev.quantity * prev.purchasePrice)) *
          100
        const currentGain =
          ((current.quantity * current.currentPrice - current.quantity * current.purchasePrice) /
            (current.quantity * current.purchasePrice)) *
          100
        return currentGain < prevGain ? current : prev
      }),
    }
  }

  const metrics = calculateMetrics()

  return (
    <Card>
      <CardHeader>
        <CardTitle>Indicadores de Desempenho</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          {metrics && (
            <>
              <div>
                <p className="text-sm text-muted-foreground mb-1">Taxa de Sucesso</p>
                <p className="text-2xl font-bold text-green-600">{metrics.winRate}%</p>
              </div>

              <div className="border-t pt-4">
                <p className="text-sm text-muted-foreground mb-2">Melhor Performance</p>
                <div className="bg-green-50 dark:bg-green-900/20 p-3 rounded-lg">
                  <p className="font-medium text-green-700 dark:text-green-300">{metrics.topGainer.symbol}</p>
                  <p className="text-sm text-green-600 dark:text-green-400">
                    +
                    {(
                      ((metrics.topGainer.currentPrice - metrics.topGainer.purchasePrice) /
                        metrics.topGainer.purchasePrice) *
                      100
                    ).toFixed(2)}
                    %
                  </p>
                </div>
              </div>

              <div className="border-t pt-4">
                <p className="text-sm text-muted-foreground mb-2">Pior Performance</p>
                <div className="bg-red-50 dark:bg-red-900/20 p-3 rounded-lg">
                  <p className="font-medium text-red-700 dark:text-red-300">{metrics.topLoser.symbol}</p>
                  <p className="text-sm text-red-600 dark:text-red-400">
                    {(
                      ((metrics.topLoser.currentPrice - metrics.topLoser.purchasePrice) /
                        metrics.topLoser.purchasePrice) *
                      100
                    ).toFixed(2)}
                    %
                  </p>
                </div>
              </div>
            </>
          )}
        </div>
      </CardContent>
    </Card>
  )
}
