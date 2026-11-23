export type FeedbackType = 'general' | 'feature' | 'bug';

export interface FeedbackSubmission {
  name: string;
  email: string;
  feedbackType: FeedbackType;
  rating: number;
  comments: string;
}

export interface FeedbackResponse {
  success: boolean;
  message: string;
  referenceId?: string;
}
