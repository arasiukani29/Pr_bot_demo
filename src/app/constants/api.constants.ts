export const API_ENDPOINTS = {
  BASE_URL: 'http://localhost:5000/api',
  AUTH: {
    LOGIN: '/api/auth/login',
    LOGOUT: '/api/auth/logout',
    REFRESH: '/api/auth/refresh',
    PROFILE: '/api/auth/profile'
  },
  USER_PROFILE: {
    GET: (username: string) => `/api/userprofile/${username}`,
    UPDATE: '/api/userprofile',
    UPLOAD_AVATAR: '/api/userprofile/avatar'
  }
} as const;
