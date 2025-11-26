"use client"

import { useState, useEffect } from "react"
import { LoginScreen } from "@/components/login-screen"
import { PortfolioDashboard } from "@/components/portfolio-dashboard"
import { AssetManager } from "@/components/asset-manager"
import { AnalysisView } from "@/components/analysis-view"
import { PortfolioProvider } from "@/context/portfolio-context"

export default function Home() {
  const [isLoggedIn, setIsLoggedIn] = useState(false)
  const [view, setView] = useState<"dashboard" | "assets" | "analysis">("dashboard")
  const [mounted, setMounted] = useState(false)

  useEffect(() => {
    const savedLogin = localStorage.getItem("isLoggedIn")
    if (savedLogin) {
      setIsLoggedIn(true)
    }
    setMounted(true)
  }, [])

  if (!mounted) return null

  if (!isLoggedIn) {
    return (
      <LoginScreen
        onLogin={() => {
          setIsLoggedIn(true)
          localStorage.setItem("isLoggedIn", "true")
        }}
      />
    )
  }

  return (
    <PortfolioProvider>
      <main className="min-h-screen bg-background">
        <Navigation
          view={view}
          setView={setView}
          onLogout={() => {
            setIsLoggedIn(false)
            localStorage.removeItem("isLoggedIn")
          }}
        />

        <div className="container mx-auto px-4 py-8">
          {view === "dashboard" && <PortfolioDashboard />}
          {view === "assets" && <AssetManager />}
          {view === "analysis" && <AnalysisView />}
        </div>
      </main>
    </PortfolioProvider>
  )
}

function Navigation({
  view,
  setView,
  onLogout,
}: {
  view: string
  setView: (v: "dashboard" | "assets" | "analysis") => void
  onLogout: () => void
}) {
  return (
    <nav className="sticky top-0 z-50 border-b border-border bg-card backdrop-blur-sm">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <h1 className="text-2xl font-bold text-primary">Investidor10</h1>

          <div className="flex gap-2">
            {[
              { id: "dashboard", label: "Dashboard" },
              { id: "assets", label: "Meus Ativos" },
              { id: "analysis", label: "Análises" },
            ].map((item) => (
              <button
                key={item.id}
                onClick={() => setView(item.id as any)}
                className={`px-4 py-2 rounded-md transition-colors ${
                  view === item.id ? "bg-primary text-primary-foreground" : "hover:bg-secondary text-foreground"
                }`}
              >
                {item.label}
              </button>
            ))}
          </div>

          <button
            onClick={onLogout}
            className="px-4 py-2 text-muted-foreground hover:text-foreground transition-colors"
          >
            Sair
          </button>
        </div>
      </div>
    </nav>
  )
}
