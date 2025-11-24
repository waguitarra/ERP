import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

interface StatCard {
  title: string;
  value: string;
  change: string;
  isPositive: boolean;
  icon: string;
  color: string;
}

interface RecentOrder {
  id: string;
  customer: string;
  product: string;
  amount: string;
  status: 'Concluído' | 'Pendente' | 'Cancelado';
  date: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
  stats: StatCard[] = [
    { 
      title: 'Vendas Totais', 
      value: 'R$ 124.500', 
      change: '+12%', 
      isPositive: true, 
      color: 'bg-blue-500',
      icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z'
    },
    { 
      title: 'Novos Pedidos', 
      value: '1,234', 
      change: '+5.4%', 
      isPositive: true, 
      color: 'bg-purple-500',
      icon: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z'
    },
    { 
      title: 'Produtos em Estoque', 
      value: '8,450', 
      change: '-2%', 
      isPositive: false, 
      color: 'bg-orange-500',
      icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4'
    },
    { 
      title: 'Clientes Ativos', 
      value: '2,842', 
      change: '+18%', 
      isPositive: true, 
      color: 'bg-emerald-500',
      icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z'
    }
  ];

  recentOrders: RecentOrder[] = [
    { id: '#1001', customer: 'Maria Silva', product: 'Produto A', amount: 'R$ 2.400', status: 'Concluído', date: 'Hoje' },
    { id: '#1002', customer: 'João Souza', product: 'Produto B', amount: 'R$ 1.800', status: 'Pendente', date: 'Hoje' },
    { id: '#1003', customer: 'Ana Costa', product: 'Produto C', amount: 'R$ 3.500', status: 'Concluído', date: 'Ontem' },
    { id: '#1004', customer: 'Pedro Alves', product: 'Produto D', amount: 'R$ 1.100', status: 'Cancelado', date: 'Ontem' },
  ];

  chartData = [
    { label: 'Jan', height: 40, value: 'R$ 40k', fill: 60 },
    { label: 'Fev', height: 65, value: 'R$ 65k', fill: 80 },
    { label: 'Mar', height: 45, value: 'R$ 45k', fill: 50 },
    { label: 'Abr', height: 80, value: 'R$ 80k', fill: 90 },
    { label: 'Mai', height: 55, value: 'R$ 55k', fill: 70 },
    { label: 'Jun', height: 90, value: 'R$ 90k', fill: 100 },
  ];

  getInitials(name: string): string {
    return name
      .split(' ')
      .map(n => n[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Concluído': return 'text-green-600';
      case 'Pendente': return 'text-amber-600';
      case 'Cancelado': return 'text-red-600';
      default: return 'text-slate-600';
    }
  }
}
