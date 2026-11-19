import { StorageService } from '../services/storage.service';

export interface HttpRequestLike {
  headers: Record<string, string>;
  url: string;
}

export function jwtInterceptor(req: HttpRequestLike, storageService: StorageService): HttpRequestLike {
  const token = storageService.getToken();


  if (token) {
    return {
      ...req,
      headers: {
        ...req.headers,
        Authorization: `Bearer ${token}`,
        'X-Debug-Auth-Token': token
      }
    };
  }

  return req;
}
