export interface UserProfile {
  username: string;
  displayName: string;
  email: string;
  bio: string;
  avatarUrl?: string;
}

export interface AvatarUploadResponse {
  success: boolean;
  message: string;
  avatarUrl?: string;
}
