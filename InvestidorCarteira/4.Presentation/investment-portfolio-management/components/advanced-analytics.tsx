"use client"

import type { Portfolio } from "@/context/portfolio-context"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from "recharts"

interface AdvancedAnalyticsProps {
  portfolio: Portfolio
}

const COLORS = ["#3b82f6", "#8b5cf6", "#f59e0b", "#10b981", "#06b6d4", "#ef4444"]

export function AdvancedAnalytics({ portfolio }: AdvancedAnalyticsProps) {
  if (portfolio.assets.length === 0) {
    return (
      <Card>
        <CardContent className="pt-6">
          <p className="text-center text-muted-foreground py-8">Adicione ativos para ver análises</p>
        </CardContent>
      </Card>
    )
  }

  // Prepare data for type allocation chart
  const typeData = portfolio.assets.reduce(
    (acc, asset) => {
      const value = asset.quantity * asset.currentPrice
      const existing = acc.find((item) => item.name === asset.type)
      if (existing) {
        existing.value += value
      } else {
        acc.push({ name: asset.type, value })
      }
      return acc
    },
    [] as Array<{ name: string; value: number }>,
  )

  // Prepare data for performance chart
  const perfData = portfolio.assets
    .sort((a, b) => {
      const aGain = ((a.currentPrice - a.purchasePrice) / a.purchasePrice) * 100
      const bGain = ((b.currentPrice - b.purchasePrice) / b.purchasePrice) * 100
      return bGain - aGain
    })
    .slice(0, 10)
    .map((asset) => ({
      symbol: asset.symbol,
      performance: (((asset.currentPrice - asset.purchasePrice) / asset.purchasePrice) * 100).toFixed(2),
    }))

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <Card>
        <CardHeader>
          <CardTitle>Alocação por Tipo</CardTitle>
        </CardHeader>
        <CardContent>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={typeData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                outerRadius={80}
                fill="#8884d8"
                dataKey="value"
              >
                {typeData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Top 10 Performance</CardTitle>
        </CardHeader>
        <CardContent>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={perfData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="symbol" />
              <YAxis />
              <Tooltip
                formatter={(value) => `${value}%`}
                contentStyle={{ backgroundColor: "var(--background)", border: "1px solid var(--border)" }}
              />
              <Bar dataKey="performance" fill="#8b5cf6" />
            </BarChart>
          </ResponsiveContainer>
        </CardContent>
      </Card>
    </div>
  )
}
