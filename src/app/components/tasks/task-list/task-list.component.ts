import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  TaskItem,
  TaskItemStatus,
  TaskPriority,
  TaskFilter,
  TaskSummary
} from '../../../models/task.model';
import { TaskService } from '../../../services/task.service';
import { TaskFormComponent } from '../task-form/task-form.component';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TaskFormComponent],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css']
})
export class TaskListComponent implements OnInit {
  tasks: TaskItem[] = [];
  summary: TaskSummary | null = null;
  isLoading = false;

  searchTerm = '';
  selectedStatus: TaskItemStatus | '' = '';
  selectedPriority: TaskPriority | '' = '';

  currentPage = 1;
  pageSize = 6;
  totalPages = 1;
  totalCount = 0;

  isModalOpen = false;
  selectedTaskForEdit: TaskItem | null = null;
  notificationMessage = '';
  notificationType: 'success' | 'error' = 'success';

  readonly statusOptions: TaskItemStatus[] = ['Todo', 'InProgress', 'InReview', 'Completed'];
  readonly priorityOptions: TaskPriority[] = ['Low', 'Medium', 'High', 'Critical'];

  constructor(private readonly taskService: TaskService) {}

  ngOnInit(): void {
    this.refreshAll();
  }

  refreshAll(): void {
    this.loadTasks();
    this.loadSummary();
  }

  loadTasks(): void {
    this.isLoading = true;
    const filter: TaskFilter = {
      searchTerm: this.searchTerm.trim() || undefined,
      status: this.selectedStatus || undefined,
      priority: this.selectedPriority || undefined,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    this.taskService.getTasks(filter).subscribe({
      next: response => {
        this.tasks = response.items;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.currentPage = response.page;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.showNotification('Failed to load tasks.', 'error');
      }
    });
  }

  loadSummary(): void {
    this.taskService.getSummary().subscribe({
      next: summary => {
        this.summary = summary;
      }
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadTasks();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadTasks();
  }

  onResetFilters(): void {
    this.searchTerm = '';
    this.selectedStatus = '';
    this.selectedPriority = '';
    this.currentPage = 1;
    this.loadTasks();
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadTasks();
    }
  }

  openCreateModal(): void {
    this.selectedTaskForEdit = null;
    this.isModalOpen = true;
  }

  openEditModal(task: TaskItem): void {
    this.selectedTaskForEdit = task;
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedTaskForEdit = null;
  }

  onTaskSaved(): void {
    const isEdit = !!this.selectedTaskForEdit;
    this.closeModal();
    this.refreshAll();
    this.showNotification(
      isEdit ? 'Task updated successfully!' : 'Task created successfully!',
      'success'
    );
  }

  onDelete(task: TaskItem): void {
    if (confirm(`Are you sure you want to delete task "${task.title}"?`)) {
      this.taskService.deleteTask(task.id).subscribe({
        next: success => {
          if (success) {
            this.showNotification('Task deleted successfully.', 'success');
            this.refreshAll();
          } else {
            this.showNotification('Could not delete task.', 'error');
          }
        }
      });
    }
  }

  onQuickStatusChange(task: TaskItem, newStatus: TaskItemStatus): void {
    if (task.status === newStatus) return;

    this.taskService.updateTask(task.id, {
      title: task.title,
      description: task.description,
      status: newStatus,
      priority: task.priority,
      assignedTo: task.assignedTo,
      dueDate: task.dueDate
    }).subscribe({
      next: updated => {
        if (updated) {
          task.status = updated.status;
          task.updatedAt = updated.updatedAt;
          this.loadSummary();
          this.showNotification(`Task moved to ${newStatus}.`, 'success');
        }
      }
    });
  }

  getPriorityClass(priority: TaskPriority): string {
    switch (priority) {
      case 'Critical':
        return 'badge-priority-critical';
      case 'High':
        return 'badge-priority-high';
      case 'Medium':
        return 'badge-priority-medium';
      case 'Low':
        return 'badge-priority-low';
      default:
        return '';
    }
  }

  getStatusClass(status: TaskItemStatus): string {
    switch (status) {
      case 'Completed':
        return 'badge-status-completed';
      case 'InProgress':
        return 'badge-status-inprogress';
      case 'InReview':
        return 'badge-status-inreview';
      case 'Todo':
        return 'badge-status-todo';
      default:
        return '';
    }
  }

  isOverdue(task: TaskItem): boolean {
    if (!task.dueDate || task.status === 'Completed') return false;
    return new Date(task.dueDate) < new Date();
  }

  private showNotification(msg: string, type: 'success' | 'error'): void {
    this.notificationMessage = msg;
    this.notificationType = type;
    setTimeout(() => {
      this.notificationMessage = '';
    }, 3500);
  }
}
