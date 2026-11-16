import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface UserItem {
  id: number;
  name: string;
  email: string;
  role: string;
  status: 'Active' | 'Pending' | 'Inactive';
}

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './table.component.html',
  styleUrl: './table.component.css'
})
export class TableComponent {
  private readonly emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  // Live reactive search query
  public searchTerm = signal<string>('');

  // Primary data table list
  public users = signal<UserItem[]>([
    { id: 1, name: 'Alice Smith', email: 'alice@example.com', role: 'Admin', status: 'Active' },
    { id: 2, name: 'Bob Jones', email: 'bob@example.com', role: 'Developer', status: 'Active' },
    { id: 3, name: 'Carol White', email: 'carol@example.com', role: 'Designer', status: 'Pending' },
    { id: 4, name: 'David Lee', email: 'david@example.com', role: 'Product Manager', status: 'Active' }
  ]);

  // Form input models for adding a new row
  public newName = '';
  public newEmail = '';
  public newRole = 'Developer';

  // Computed signal that automatically filters table rows based on search term
  public filteredUsers = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    if (!term) {
      return this.users();
    }
    return this.users().filter(user =>
      user.name.toLowerCase().includes(term) ||
      user.email.toLowerCase().includes(term) ||
      user.role.toLowerCase().includes(term)
    );
  });

  // Action to add a new row to the table with email format validation & safe ID reduction
  public addUser(): void {
    const trimmedName = this.newName.trim();
    const trimmedEmail = this.newEmail.trim();

    if (!trimmedName || !trimmedEmail || !this.emailRegex.test(trimmedEmail)) {
      return;
    }

    const nextId = Math.max(0, ...this.users().map(u => u.id)) + 1;

    this.users.update(currentList => [
      ...currentList,
      {
        id: nextId,
        name: trimmedName,
        email: trimmedEmail,
        role: this.newRole,
        status: 'Active'
      }
    ]);

    // Reset form fields
    this.newName = '';
    this.newEmail = '';
    this.newRole = 'Developer';
  }

  // Action to delete a row from the table
  public deleteUser(id: number): void {
    this.users.update(currentList => currentList.filter(user => user.id !== id));
  }
}
