"use client"

import type React from "react"

import { useState } from "react"
import { usePortfolio, type Asset } from "@/context/portfolio-context"
import { Button } from "@/components/ui/button"

interface AssetFormProps {
  asset?: Asset
  onClose: () => void
}

const ASSET_TYPES = [
  { value: "stock", label: "Ação" },
  { value: "etf", label: "ETF" },
  { value: "crypto", label: "Criptomoeda" },
  { value: "fund", label: "Fundo" },
  { value: "fixed-income", label: "Renda Fixa" },
  { value: "real-estate", label: "Imóvel" },
]

export function AssetForm({ asset, onClose }: AssetFormProps) {
  const { addAsset, updateAsset } = usePortfolio()
  const [formData, setFormData] = useState({
    name: asset?.name || "",
    symbol: asset?.symbol || "",
    type: (asset?.type || "stock") as Asset["type"],
    quantity: asset?.quantity || 0,
    purchasePrice: asset?.purchasePrice || 0,
    currentPrice: asset?.currentPrice || 0,
    purchaseDate: asset?.purchaseDate || new Date().toISOString().split("T")[0],
    sector: asset?.sector || "",
    notes: asset?.notes || "",
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()

    if (!formData.name || !formData.symbol || formData.quantity <= 0 || formData.purchasePrice <= 0) {
      alert("Por favor, preencha todos os campos obrigatórios")
      return
    }

    if (asset) {
      updateAsset(asset.id, formData)
    } else {
      addAsset(formData)
    }

    onClose()
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]:
        name === "quantity" || name === "purchasePrice" || name === "currentPrice"
          ? Number.parseFloat(value) || 0
          : value,
    }))
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium mb-1">Nome do Ativo *</label>
          <input
            type="text"
            name="name"
            value={formData.name}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-border rounded-md bg-background"
            placeholder="Ex: Apple Inc"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Símbolo *</label>
          <input
            type="text"
            name="symbol"
            value={formData.symbol}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-border rounded-md bg-background"
            placeholder="Ex: AAPL"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Tipo de Ativo *</label>
          <select
            name="type"
            value={formData.type}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-border rounded-md bg-background"
          >
            {ASSET_TYPES.map((type) => (
              <option key={type.value} value={type.value}>
                {type.label}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Data de Compra *</label>
          <input
            type="date"
            name="purchaseDate"
            value={formData.purchaseDate}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-border rounded-md bg-background"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Quantidade *</label>
          <input
            type="number"
            name="quantity"
            value={formData.quantity}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-border rounded-md bg-background"
            placeholder="0"
            step="0.01"
            min="0"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Preço de Compra (R$) *</label>
          <input
            type="number"
            name="purchasePrice"
            value={formData.purchasePrice}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-border rounded-md bg-background"
            placeholder="0.00"
            step="0.01"
            min="0"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Preço Atual (R$) *</label>
          <input
            type="number"
            name="currentPrice"
            value={formData.currentPrice}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-border rounded-md bg-background"
            placeholder="0.00"
            step="0.01"
            min="0"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Setor</label>
          <input
            type="text"
            name="sector"
            value={formData.sector}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-border rounded-md bg-background"
            placeholder="Ex: Tecnologia"
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium mb-1">Notas</label>
        <textarea
          name="notes"
          value={formData.notes}
          onChange={handleChange}
          className="w-full px-3 py-2 border border-border rounded-md bg-background"
          rows={3}
          placeholder="Observações sobre este ativo..."
        />
      </div>

      <div className="flex gap-2 justify-end">
        <Button type="button" variant="outline" onClick={onClose}>
          Cancelar
        </Button>
        <Button type="submit">{asset ? "Atualizar" : "Adicionar"} Ativo</Button>
      </div>
    </form>
  )
}
