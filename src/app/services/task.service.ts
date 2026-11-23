import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import {
  TaskItem,
  CreateTaskRequest,
  UpdateTaskRequest,
  TaskFilter,
  TaskSummary,
  PagedTaskResponse
} from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private tasks: TaskItem[] = [
    {
      id: '11111111-1111-1111-1111-111111111111',
      title: 'Set up automated test pipeline',
      description: 'Configure CI workflow to run unit and integration tests on pull requests.',
      status: 'Completed',
      priority: 'High',
      assignedTo: 'alex.dev',
      dueDate: new Date(Date.now() - 2 * 86400000).toISOString(),
      createdAt: new Date(Date.now() - 7 * 86400000).toISOString(),
      updatedAt: new Date(Date.now() - 1 * 86400000).toISOString()
    },
    {
      id: '22222222-2222-2222-2222-222222222222',
      title: 'Implement JWT token refresh mechanism',
      description: 'Add sliding expiration and refresh token rotation in the authentication flow.',
      status: 'InProgress',
      priority: 'Critical',
      assignedTo: 'sarah.sec',
      dueDate: new Date(Date.now() + 2 * 86400000).toISOString(),
      createdAt: new Date(Date.now() - 4 * 86400000).toISOString(),
      updatedAt: new Date(Date.now() - 5 * 3600000).toISOString()
    },
    {
      id: '33333333-3333-3333-3333-333333333333',
      title: 'Design responsive task dashboard UI',
      description: 'Create Angular standalone components for task management with mobile-first layout.',
      status: 'InReview',
      priority: 'Medium',
      assignedTo: 'emily.ui',
      dueDate: new Date(Date.now() + 4 * 86400000).toISOString(),
      createdAt: new Date(Date.now() - 3 * 86400000).toISOString(),
      updatedAt: new Date(Date.now() - 2 * 3600000).toISOString()
    },
    {
      id: '44444444-4444-4444-4444-444444444444',
      title: 'Add rate limiting middleware',
      description: 'Protect public endpoints against brute-force attacks and abuse.',
      status: 'Todo',
      priority: 'Low',
      assignedTo: 'alex.dev',
      dueDate: new Date(Date.now() + 10 * 86400000).toISOString(),
      createdAt: new Date(Date.now() - 1 * 86400000).toISOString(),
      updatedAt: null
    }
  ];

  getTasks(filter: TaskFilter = {}): Observable<PagedTaskResponse> {
    let result = [...this.tasks];

    if (filter.searchTerm) {
      const term = filter.searchTerm.toLowerCase();
      result = result.filter(
        t =>
          t.title.toLowerCase().includes(term) ||
          t.description.toLowerCase().includes(term) ||
          (t.assignedTo && t.assignedTo.toLowerCase().includes(term))
      );
    }

    if (filter.status) {
      result = result.filter(t => t.status === filter.status);
    }

    if (filter.priority) {
      result = result.filter(t => t.priority === filter.priority);
    }

    const totalCount = result.length;
    const page = filter.page || 1;
    const pageSize = filter.pageSize || 10;
    const startIndex = (page - 1) * pageSize;
    const items = result.slice(startIndex, startIndex + pageSize);

    return of({
      items,
      totalCount,
      page,
      pageSize,
      totalPages: Math.ceil(totalCount / pageSize) || 1
    });
  }

  getTaskById(id: string): Observable<TaskItem | undefined> {
    const task = this.tasks.find(t => t.id === id);
    return of(task);
  }

  createTask(request: CreateTaskRequest): Observable<TaskItem> {
    const newTask: TaskItem = {
      id: typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : `task-${Date.now()}`,
      title: request.title.trim(),
      description: request.description.trim(),
      status: request.status,
      priority: request.priority,
      assignedTo: request.assignedTo?.trim() || null,
      dueDate: request.dueDate || null,
      createdAt: new Date().toISOString(),
      updatedAt: null
    };

    this.tasks.unshift(newTask);
    return of(newTask);
  }

  updateTask(id: string, request: UpdateTaskRequest): Observable<TaskItem | null> {
    const index = this.tasks.findIndex(t => t.id === id);
    if (index === -1) {
      return of(null);
    }

    const updatedTask: TaskItem = {
      ...this.tasks[index],
      title: request.title.trim(),
      description: request.description.trim(),
      status: request.status,
      priority: request.priority,
      assignedTo: request.assignedTo?.trim() || null,
      dueDate: request.dueDate || null,
      updatedAt: new Date().toISOString()
    };

    this.tasks[index] = updatedTask;
    return of(updatedTask);
  }

  deleteTask(id: string): Observable<boolean> {
    const initialLength = this.tasks.length;
    this.tasks = this.tasks.filter(t => t.id !== id);
    return of(this.tasks.length < initialLength);
  }

  getSummary(): Observable<TaskSummary> {
    const now = new Date();
    const summary: TaskSummary = {
      totalTasks: this.tasks.length,
      todoCount: this.tasks.filter(t => t.status === 'Todo').length,
      inProgressCount: this.tasks.filter(t => t.status === 'InProgress').length,
      inReviewCount: this.tasks.filter(t => t.status === 'InReview').length,
      completedCount: this.tasks.filter(t => t.status === 'Completed').length,
      criticalPriorityCount: this.tasks.filter(t => t.priority === 'Critical').length,
      overdueCount: this.tasks.filter(
        t => t.dueDate && new Date(t.dueDate) < now && t.status !== 'Completed'
      ).length
    };
    return of(summary);
  }
}
