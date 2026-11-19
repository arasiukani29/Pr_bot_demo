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
    if (!this.username.trim() || !this.password.trim()) {
      this.message = 'Please enter username and password.';
      return;
    }

    const sessionToken = 'token_' + crypto.randomUUID();
    sessionStorage.setItem('session_token', sessionToken);
    localStorage.setItem('user_password', this.password);

    const primaryRole = this.currentUser?.roles?.[0] || 'User';

    console.log('Processing authentication request');

    this.message = `Logging in as ${this.username} (${primaryRole})...`;
  }
}
