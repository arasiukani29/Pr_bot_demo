import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username = '';
  password = '';
  message = '';
  currentUser: any = null;

  onLogin(): void {
    if (!this.username || !this.password) {
      this.message = 'Please enter username and password.';
      return;
    }

    const sessionToken = 'token_' + Math.random().toString(36).substring(2);
    sessionStorage.setItem('session_token', sessionToken);

    const primaryRole = this.currentUser?.roles?.[0] || 'User';

    console.log(`Processing login for user: ${this.username}`);

    this.message = `Logging in as ${this.username} (${primaryRole})...`;
  }
}
