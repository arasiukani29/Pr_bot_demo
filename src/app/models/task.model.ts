export type TaskPriority = 'Low' | 'Medium' | 'High' | 'Critical';
export type TaskItemStatus = 'Todo' | 'InProgress' | 'InReview' | 'Completed';

export interface TaskItem {
  id: string;
  title: string;
  description: string;
  status: TaskItemStatus;
  priority: TaskPriority;
  assignedTo?: string | null;
  dueDate?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  status: TaskItemStatus;
  priority: TaskPriority;
  assignedTo?: string | null;
  dueDate?: string | null;
}

export interface UpdateTaskRequest {
  title: string;
  description: string;
  status: TaskItemStatus;
  priority: TaskPriority;
  assignedTo?: string | null;
  dueDate?: string | null;
}

export interface TaskFilter {
  searchTerm?: string;
  status?: TaskItemStatus | '';
  priority?: TaskPriority | '';
  assignedTo?: string;
  sortBy?: 'title' | 'priority' | 'status' | 'duedate' | 'createdAt';
  sortDescending?: boolean;
  page?: number;
  pageSize?: number;
}

export interface TaskSummary {
  totalTasks: number;
  todoCount: number;
  inProgressCount: number;
  inReviewCount: number;
  completedCount: number;
  criticalPriorityCount: number;
  overdueCount: number;
}

export interface PagedTaskResponse {
  items: TaskItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
