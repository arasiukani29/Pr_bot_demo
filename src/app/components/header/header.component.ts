import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  title = 'PR Bot Demo App';
  isLoggedIn = false;

  onLogout(): void {
    this.isLoggedIn = false;
  }
}
