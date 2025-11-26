"use client"

import { useState } from "react"
import { usePortfolio } from "@/context/portfolio-context"
import { PortfolioOverview } from "./portfolio-overview"
import { AssetAllocation } from "./asset-allocation"
import { PerformanceMetrics } from "./performance-metrics"
import { TopAssets } from "./top-assets"
import { AddAssetModal } from "./add-asset-modal"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"

export function PortfolioDashboard() {
  const { portfolio } = usePortfolio()
  const [showAddModal, setShowAddModal] = useState(false)

  if (portfolio.assets.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[60vh] text-center">
        <h2 className="text-2xl font-bold mb-2">Sua carteira está vazia</h2>
        <p className="text-muted-foreground mb-6">Comece adicionando seus primeiros ativos para ver o dashboard</p>
        <Button onClick={() => setShowAddModal(true)} className="gap-2">
          <Plus className="w-4 h-4" />
          Adicionar Primeiro Ativo
        </Button>
        <AddAssetModal isOpen={showAddModal} onClose={() => setShowAddModal(false)} />
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-end">
        <Button onClick={() => setShowAddModal(true)} className="gap-2">
          <Plus className="w-4 h-4" />
          Adicionar Ativo
        </Button>
      </div>

      <PortfolioOverview portfolio={portfolio} />

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <AssetAllocation assets={portfolio.assets} />
        <PerformanceMetrics portfolio={portfolio} />
      </div>

      <TopAssets assets={portfolio.assets} />

      <AddAssetModal isOpen={showAddModal} onClose={() => setShowAddModal(false)} />
    </div>
  )
}
