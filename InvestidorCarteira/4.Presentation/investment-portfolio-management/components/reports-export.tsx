"use client"

import type { Portfolio } from "@/context/portfolio-context"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Download, FileText } from "lucide-react"

interface ReportsExportProps {
  portfolio: Portfolio
}

export function ReportsExport({ portfolio }: ReportsExportProps) {
  const exportAsCSV = () => {
    if (portfolio.assets.length === 0) {
      alert("Nenhum ativo para exportar")
      return
    }

    const headers = [
      "Símbolo",
      "Nome",
      "Tipo",
      "Quantidade",
      "Preço Compra",
      "Preço Atual",
      "Valor Total",
      "Ganho",
      "Rentabilidade %",
    ]

    const rows = portfolio.assets.map((asset) => {
      const totalValue = asset.quantity * asset.currentPrice
      const totalCost = asset.quantity * asset.purchasePrice
      const gain = totalValue - totalCost
      const gainPercent = totalCost > 0 ? (gain / totalCost) * 100 : 0

      return [
        asset.symbol,
        asset.name,
        asset.type,
        asset.quantity,
        asset.purchasePrice.toFixed(2),
        asset.currentPrice.toFixed(2),
        totalValue.toFixed(2),
        gain.toFixed(2),
        gainPercent.toFixed(2),
      ]
    })

    const csvContent = [headers.join(","), ...rows.map((row) => row.map((cell) => `"${cell}"`).join(","))].join("\n")

    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" })
    const link = document.createElement("a")
    link.href = URL.createObjectURL(blob)
    link.download = `portfolio_${new Date().toISOString().split("T")[0]}.csv`
    link.click()
  }

  const exportAsJSON = () => {
    if (portfolio.assets.length === 0) {
      alert("Nenhum ativo para exportar")
      return
    }

    const exportData = {
      exportDate: new Date().toISOString(),
      totalInvested: portfolio.totalInvested,
      currentValue: portfolio.currentValue,
      totalGain: portfolio.totalGain,
      totalGainPercent: portfolio.totalGainPercent,
      assets: portfolio.assets.map((asset) => ({
        ...asset,
        totalValue: asset.quantity * asset.currentPrice,
        gain: asset.quantity * asset.currentPrice - asset.quantity * asset.purchasePrice,
      })),
    }

    const blob = new Blob([JSON.stringify(exportData, null, 2)], { type: "application/json" })
    const link = document.createElement("a")
    link.href = URL.createObjectURL(blob)
    link.download = `portfolio_${new Date().toISOString().split("T")[0]}.json`
    link.click()
  }

  const generateReport = () => {
    if (portfolio.assets.length === 0) {
      alert("Nenhum ativo para gerar relatório")
      return
    }

    const reportDate = new Date().toLocaleDateString("pt-BR")
    const typeGroups = portfolio.assets.reduce(
      (acc, asset) => {
        acc[asset.type] = (acc[asset.type] || 0) + asset.quantity * asset.currentPrice
        return acc
      },
      {} as Record<string, number>,
    )

    const reportContent = `
RELATÓRIO DE CARTEIRA DE INVESTIMENTOS
${"=".repeat(50)}
Data do Relatório: ${reportDate}
Moeda: ${portfolio.currency}

RESUMO EXECUTIVO
${"=".repeat(50)}
Valor Total Investido: R$ ${portfolio.totalInvested.toFixed(2)}
Valor Atual da Carteira: R$ ${portfolio.currentValue.toFixed(2)}
Ganho Total: R$ ${portfolio.totalGain.toFixed(2)}
Rentabilidade: ${portfolio.totalGainPercent.toFixed(2)}%

ALOCAÇÃO POR TIPO DE ATIVO
${"=".repeat(50)}
${Object.entries(typeGroups)
  .map(([type, value]) => {
    const percent = (value / portfolio.currentValue) * 100
    return `${type.toUpperCase()}: R$ ${value.toFixed(2)} (${percent.toFixed(1)}%)`
  })
  .join("\n")}

DETALHES DOS ATIVOS
${"=".repeat(50)}
${portfolio.assets
  .map((asset) => {
    const totalValue = asset.quantity * asset.currentPrice
    const totalCost = asset.quantity * asset.purchasePrice
    const gain = totalValue - totalCost
    const gainPercent = totalCost > 0 ? (gain / totalCost) * 100 : 0
    return `
${asset.symbol} - ${asset.name}
  Tipo: ${asset.type}
  Quantidade: ${asset.quantity}
  Preço de Compra: R$ ${asset.purchasePrice.toFixed(2)}
  Preço Atual: R$ ${asset.currentPrice.toFixed(2)}
  Valor Total: R$ ${totalValue.toFixed(2)}
  Ganho: R$ ${gain.toFixed(2)} (${gainPercent.toFixed(2)}%)
${asset.notes ? `  Notas: ${asset.notes}` : ""}
    `
  })
  .join("\n")}

ANÁLISE DE PERFORMANCE
${"=".repeat(50)}
Total de Ativos: ${portfolio.assets.length}
Ativos com Ganho: ${portfolio.assets.filter((a) => a.quantity * a.currentPrice > a.quantity * a.purchasePrice).length}
Ativos com Perda: ${portfolio.assets.filter((a) => a.quantity * a.currentPrice < a.quantity * a.purchasePrice).length}
Tax de Sucesso: ${((portfolio.assets.filter((a) => a.quantity * a.currentPrice > a.quantity * a.purchasePrice).length / portfolio.assets.length) * 100).toFixed(1)}%

Gerado por: Portfolio Pro
Data de Geração: ${new Date().toLocaleString("pt-BR")}
    `.trim()

    const blob = new Blob([reportContent], { type: "text/plain;charset=utf-8;" })
    const link = document.createElement("a")
    link.href = URL.createObjectURL(blob)
    link.download = `relatorio_portfolio_${new Date().toISOString().split("T")[0]}.txt`
    link.click()
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <FileText className="w-5 h-5" />
          Relatórios e Exportação
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-3">
          <p className="text-sm text-muted-foreground">
            Exporte e gere relatórios de sua carteira para análise offline ou integração com outras ferramentas.
          </p>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
            <Button
              onClick={exportAsCSV}
              variant="outline"
              className="gap-2 bg-transparent"
              disabled={portfolio.assets.length === 0}
            >
              <Download className="w-4 h-4" />
              Exportar CSV
            </Button>

            <Button
              onClick={exportAsJSON}
              variant="outline"
              className="gap-2 bg-transparent"
              disabled={portfolio.assets.length === 0}
            >
              <Download className="w-4 h-4" />
              Exportar JSON
            </Button>

            <Button
              onClick={generateReport}
              variant="outline"
              className="gap-2 bg-transparent"
              disabled={portfolio.assets.length === 0}
            >
              <Download className="w-4 h-4" />
              Gerar Relatório
            </Button>
          </div>

          <div className="bg-secondary/50 rounded-lg p-4 text-sm space-y-2">
            <p className="font-medium">Formatos Disponíveis:</p>
            <ul className="list-disc list-inside space-y-1 text-muted-foreground">
              <li>
                <strong>CSV</strong> - Para importar em Excel ou Google Sheets
              </li>
              <li>
                <strong>JSON</strong> - Para backup completo dos dados
              </li>
              <li>
                <strong>Relatório TXT</strong> - Documento formatado para análise
              </li>
            </ul>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
