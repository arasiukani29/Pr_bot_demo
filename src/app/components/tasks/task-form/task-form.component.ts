import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskItem, TaskPriority, TaskItemStatus, CreateTaskRequest, UpdateTaskRequest } from '../../../models/task.model';
import { TaskService } from '../../../services/task.service';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.css']
})
export class TaskFormComponent implements OnInit, OnChanges {
  @Input() task: TaskItem | null = null;
  @Output() saved = new EventEmitter<boolean>();
  @Output() cancelled = new EventEmitter<void>();

  title = '';
  description = '';
  status: TaskItemStatus = 'Todo';
  priority: TaskPriority = 'Medium';
  assignedTo = '';
  dueDate = '';

  isSubmitting = false;
  errorMessage = '';

  readonly statusOptions: TaskItemStatus[] = ['Todo', 'InProgress', 'InReview', 'Completed'];
  readonly priorityOptions: TaskPriority[] = ['Low', 'Medium', 'High', 'Critical'];

  constructor(private readonly taskService: TaskService) {}

  ngOnInit(): void {
    this.populateForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['task']) {
      this.populateForm();
    }
  }

  get isEditMode(): boolean {
    return !!this.task;
  }

  private populateForm(): void {
    if (this.task) {
      this.title = this.task.title;
      this.description = this.task.description || '';
      this.status = this.task.status;
      this.priority = this.task.priority;
      this.assignedTo = this.task.assignedTo || '';
      this.dueDate = this.task.dueDate ? this.task.dueDate.substring(0, 10) : '';
    } else {
      this.resetForm();
    }
    this.errorMessage = '';
  }

  private resetForm(): void {
    this.title = '';
    this.description = '';
    this.status = 'Todo';
    this.priority = 'Medium';
    this.assignedTo = '';
    this.dueDate = '';
  }

  onSubmit(): void {
    const trimmedTitle = this.title.trim();

    if (!trimmedTitle) {
      this.errorMessage = 'Task title is required.';
      return;
    }

    if (trimmedTitle.length < 3 || trimmedTitle.length > 120) {
      this.errorMessage = 'Task title must be between 3 and 120 characters.';
      return;
    }

    if (this.description && this.description.length > 1000) {
      this.errorMessage = 'Description cannot exceed 1000 characters.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const payload: CreateTaskRequest | UpdateTaskRequest = {
      title: trimmedTitle,
      description: this.description.trim(),
      status: this.status,
      priority: this.priority,
      assignedTo: this.assignedTo.trim() || null,
      dueDate: this.dueDate ? new Date(this.dueDate).toISOString() : null
    };

    if (this.isEditMode && this.task) {
      this.taskService.updateTask(this.task.id, payload).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.saved.emit(true);
        },
        error: () => {
          this.isSubmitting = false;
          this.errorMessage = 'Failed to update task. Please try again.';
        }
      });
    } else {
      this.taskService.createTask(payload).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.saved.emit(true);
        },
        error: () => {
          this.isSubmitting = false;
          this.errorMessage = 'Failed to create task. Please try again.';
        }
      });
    }
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
