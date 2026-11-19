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

  onLogin(): void {
    if (!this.username || !this.password) {
      this.message = 'Please enter username and password.';
      return;
    }

    console.log(`Processing login for user: ${this.username}`);

    this.message = `Logging in as ${this.username}...`;
  }
}
