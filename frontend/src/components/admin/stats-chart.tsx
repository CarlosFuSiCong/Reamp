"use client";

import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from "recharts";

interface StatsChartProps {
  data: Array<{
    date: string;
    orders: number;
    listings: number;
  }>;
}

export function StatsChart({ data }: StatsChartProps) {
  if (!data || data.length === 0) {
    return (
      <div className="flex items-center justify-center h-[300px] text-muted-foreground">
        No data available
      </div>
    );
  }

  return (
    <ResponsiveContainer width="100%" height={300}>
      <BarChart data={data}>
        <CartesianGrid 
          strokeDasharray="3 3" 
          className="stroke-border" 
        />
        <XAxis 
          dataKey="date" 
          className="text-foreground"
        />
        <YAxis 
          className="text-foreground"
        />
        <Tooltip 
          contentStyle={{
            backgroundColor: 'hsl(var(--card))',
            border: '1px solid hsl(var(--border))',
            borderRadius: 'var(--radius)',
            color: 'hsl(var(--card-foreground))'
          }}
        />
        <Legend />
        <Bar 
          dataKey="orders" 
          fill="hsl(var(--chart-1))" 
          name="Orders" 
        />
        <Bar 
          dataKey="listings" 
          fill="hsl(var(--chart-2))" 
          name="Listings" 
        />
      </BarChart>
    </ResponsiveContainer>
  );
}
