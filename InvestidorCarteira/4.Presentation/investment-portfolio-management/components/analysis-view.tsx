"use client"

import { usePortfolio } from "@/context/portfolio-context"
import { ReportsExport } from "./reports-export"
import { AdvancedAnalytics } from "./advanced-analytics"

export function AnalysisView() {
  const { portfolio } = usePortfolio()

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold mb-1">Análises e Relatórios</h2>
        <p className="text-muted-foreground">Visualize análises detalhadas e exporte dados de sua carteira</p>
      </div>

      <AdvancedAnalytics portfolio={portfolio} />
      <ReportsExport portfolio={portfolio} />
    </div>
  )
}
