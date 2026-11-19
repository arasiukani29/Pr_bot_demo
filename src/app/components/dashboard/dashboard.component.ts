import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface DashboardStat {
  label: string;
  value: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styles: [`
    .dashboard-container {
      max-width: 800px;
      margin: 30px auto;
      padding: 20px;
      font-family: Arial, sans-serif;
    }
    .stats-grid {
      display: flex;
      gap: 16px;
      margin-top: 20px;
    }
    .stat-card {
      flex: 1;
      padding: 16px;
      background-color: #f3f4f6;
      border-radius: 8px;
      text-align: center;
      border: 1px solid #e5e7eb;
    }
    .stat-card h3 {
      margin: 0;
      font-size: 28px;
      color: #2563eb;
    }
    .stat-card p {
      margin: 8px 0 0;
      color: #6b7280;
    }
  `]
})
export class DashboardComponent {
  userName = 'Admin';
  stats: DashboardStat[] = [
    { label: 'Active Users', value: 128 },
    { label: 'Total PRs', value: 45 },
    { label: 'Pending Reviews', value: 3 }
  ];
}
