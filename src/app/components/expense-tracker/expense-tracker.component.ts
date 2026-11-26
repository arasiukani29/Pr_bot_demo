import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ExpenseService } from '../../services/expense.service';
import { Expense } from '../../models/expense.model';

@Component({
  selector: 'app-expense-tracker',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './expense-tracker.component.html',
  styleUrls: ['./expense-tracker.component.css']
})
export class ExpenseTrackerComponent implements OnInit {
  expenses: Expense[] = [];
  filteredExpenses: Expense[] = [];
  expenseForm!: FormGroup;
  isEditing = false;
  editingId: number | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  searchTerm = '';
  selectedCategory = '';

  readonly categories = ['Food', 'Office', 'Software', 'Travel', 'Utilities', 'Other'];

  constructor(private expenseService: ExpenseService, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initForm();
    this.loadExpenses();
  }

  private initForm(): void {
    this.expenseForm = this.fb.group({
      title:       ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      amount:      [null, [Validators.required, Validators.min(0.01), Validators.max(1_000_000)]],
      category:    ['', Validators.required],
      description: [''],
      createdBy:   ['user']
    });
  }

  loadExpenses(): void {
    this.isLoading = true;
    this.expenseService.getAll(this.selectedCategory || undefined, this.searchTerm || undefined)
      .subscribe({
        next: (data) => {
          this.expenses = data;
          this.filteredExpenses = data;
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = 'Failed to load expenses.';
          this.isLoading = false;
        }
      });
  }

  onSearch(value: string): void {
    this.searchTerm = value;
    this.loadExpenses();
  }

  onCategoryFilter(value: string): void {
    this.selectedCategory = value;
    this.loadExpenses();
  }

  onSubmit(): void {
    if (this.expenseForm.invalid) return;

    const payload = this.expenseForm.value;
    this.isLoading = true;

    if (this.isEditing && this.editingId !== null) {
      this.expenseService.update(this.editingId, payload).subscribe({
        next: () => {
          this.successMessage = 'Expense updated successfully.';
          this.resetForm();
          this.loadExpenses();
        },
        error: () => { this.errorMessage = 'Failed to update expense.'; this.isLoading = false; }
      });
    } else {
      this.expenseService.create(payload).subscribe({
        next: () => {
          this.successMessage = 'Expense created successfully.';
          this.resetForm();
          this.loadExpenses();
        },
        error: () => { this.errorMessage = 'Failed to create expense.'; this.isLoading = false; }
      });
    }
  }

  onEdit(expense: Expense): void {
    this.isEditing = true;
    this.editingId = expense.id;
    this.expenseForm.patchValue({
      title:       expense.title,
      amount:      expense.amount,
      category:    expense.category,
      description: expense.description,
      createdBy:   expense.createdBy
    });
  }

  onDelete(id: number): void {
    if (!confirm('Are you sure you want to delete this expense?')) return;
    this.expenseService.delete(id).subscribe({
      next: () => {
        this.successMessage = 'Expense deleted.';
        this.loadExpenses();
      },
      error: () => { this.errorMessage = 'Failed to delete expense.'; }
    });
  }

  resetForm(): void {
    this.expenseForm.reset({ createdBy: 'user' });
    this.isEditing = false;
    this.editingId = null;
  }

  get totalAmount(): number {
    return this.filteredExpenses.reduce((sum, e) => sum + e.amount, 0);
  }
}
