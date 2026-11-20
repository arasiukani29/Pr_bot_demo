import { Injectable } from '@angular/core';
import { UserProfile, AvatarUploadResponse } from '../models/user-profile.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private profile: UserProfile = {
    username: 'admin',
    displayName: 'System Administrator',
    email: 'admin@example.com',
    bio: 'Managing core system configurations and user access controls.',
    avatarUrl: 'https://via.placeholder.com/150'
  };

  getProfile(username: string): UserProfile {
    return { ...this.profile };
  }

  updateProfile(updated: UserProfile): void {
    this.profile = { ...updated };
  }

  uploadAvatar(file: File): Promise<AvatarUploadResponse> {
    return new Promise((resolve) => {
      const reader = new FileReader();
      reader.onload = () => {
        const dataUrl = reader.result as string;
        this.profile.avatarUrl = dataUrl;
        resolve({
          success: true,
          message: 'Avatar updated successfully.',
          avatarUrl: dataUrl
        });
      };
      reader.readAsDataURL(file);
    });
  }
}
