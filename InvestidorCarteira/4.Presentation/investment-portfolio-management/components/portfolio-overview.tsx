"use client"

import type { Portfolio } from "@/context/portfolio-context"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { TrendingUp, TrendingDown, DollarSign, Target } from "lucide-react"

interface PortfolioOverviewProps {
  portfolio: Portfolio
}

export function PortfolioOverview({ portfolio }: PortfolioOverviewProps) {
  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: portfolio.currency,
    }).format(value)
  }

  const metrics = [
    {
      title: "Valor Total",
      value: formatCurrency(portfolio.currentValue),
      icon: DollarSign,
      color: "bg-orange-100/20 text-orange-500",
    },
    {
      title: "Investido",
      value: formatCurrency(portfolio.totalInvested),
      icon: Target,
      color: "bg-blue-100/20 text-blue-500",
    },
    {
      title: "Ganho/Perda",
      value: formatCurrency(portfolio.totalGain),
      icon: portfolio.totalGain >= 0 ? TrendingUp : TrendingDown,
      color: portfolio.totalGain >= 0 ? "bg-green-100/20 text-green-500" : "bg-red-100/20 text-red-500",
    },
    {
      title: "Rentabilidade",
      value: `${portfolio.totalGainPercent.toFixed(2)}%`,
      icon: TrendingUp,
      color:
        portfolio.totalGainPercent >= 0 ? "bg-emerald-100/20 text-emerald-500" : "bg-orange-100/20 text-orange-500",
    },
  ]

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      {metrics.map((metric, index) => {
        const Icon = metric.icon
        return (
          <Card key={index} className="bg-card border-border hover:border-primary/50 transition-colors">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">{metric.title}</CardTitle>
              <div className={`p-2 rounded-lg ${metric.color}`}>
                <Icon className="w-4 h-4" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{metric.value}</div>
            </CardContent>
          </Card>
        )
      })}
    </div>
  )
}
