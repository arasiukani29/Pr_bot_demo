import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserProfile } from '../../models/user-profile.model';
import { ProfileService } from '../../services/profile.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent {
  profile: UserProfile;
  selectedFile: File | null = null;
  statusMessage = '';
  isUploading = false;

  constructor(private profileService: ProfileService) {
    this.profile = this.profileService.getProfile('admin');
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement | null;
    const files = input?.files;
    if (files && files.length > 0) {
      this.selectedFile = files[0];
      this.statusMessage = `Selected file: ${this.selectedFile?.name}`;
    }
  }

  async onUploadAvatar(): Promise<void> {
    if (!this.selectedFile) {
      this.statusMessage = 'Please select an image file first.';
      return;
    }

    this.isUploading = true;
    try {
      const response = await this.profileService.uploadAvatar(this.selectedFile);
      if (response.success && response.avatarUrl) {
        this.profile.avatarUrl = response.avatarUrl;
        this.statusMessage = 'Avatar updated successfully.';
      }
    } catch {
      this.statusMessage = 'Failed to upload avatar image.';
    } finally {
      this.isUploading = false;
    }
  }

  onSaveProfile(): void {
    if (!this.profile.displayName.trim() || !this.profile.email.trim()) {
      this.statusMessage = 'Display name and email are required.';
      return;
    }

    this.profileService.updateProfile(this.profile);
    this.statusMessage = 'Profile information saved.';
  }
}
