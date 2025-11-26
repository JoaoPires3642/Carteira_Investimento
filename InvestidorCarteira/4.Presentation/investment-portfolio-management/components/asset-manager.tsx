"use client"

import { useState } from "react"
import { usePortfolio } from "@/context/portfolio-context"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { AssetForm } from "./asset-form"
import { AssetList } from "./asset-list"
import { Plus } from "lucide-react"

export function AssetManager() {
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const { portfolio, updateAsset, deleteAsset } = usePortfolio()

  const editingAsset = editingId ? portfolio.assets.find((a) => a.id === editingId) : null

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-3xl font-bold mb-1">Gerenciar Ativos</h2>
          <p className="text-muted-foreground">Total de ativos: {portfolio.assets.length}</p>
        </div>
        <Button
          onClick={() => {
            setEditingId(null)
            setShowForm(!showForm)
          }}
          className="gap-2"
        >
          <Plus className="w-4 h-4" />
          Novo Ativo
        </Button>
      </div>

      {showForm && (
        <Card className="border-primary/50 bg-primary/5">
          <CardHeader>
            <CardTitle>{editingId ? "Editar Ativo" : "Adicionar Novo Ativo"}</CardTitle>
          </CardHeader>
          <CardContent>
            <AssetForm
              asset={editingAsset}
              onClose={() => {
                setShowForm(false)
                setEditingId(null)
              }}
            />
          </CardContent>
        </Card>
      )}

      <AssetList
        assets={portfolio.assets}
        onEdit={(id) => {
          setEditingId(id)
          setShowForm(true)
        }}
        onDelete={deleteAsset}
      />
    </div>
  )
}
