import { Component, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FeedbackSubmission, FeedbackType } from '../../models/feedback.model';

@Component({
  selector: 'app-feedback',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.css']
})
export class FeedbackComponent {
  @ViewChild('previewBox') previewBox?: ElementRef;

  feedback: FeedbackSubmission = {
    name: '',
    email: '',
    feedbackType: 'general',
    rating: 5,
    comments: ''
  };

  maxCommentLength = 500;
  isSubmitting = false;
  statusMessage = '';
  isSuccess = false;

  readonly feedbackTypes: { value: FeedbackType; label: string }[] = [
    { value: 'general', label: 'General Feedback' },
    { value: 'feature', label: 'Feature Request' },
    { value: 'bug', label: 'Bug Report' }
  ];

  readonly ratingOptions: number[] = [1, 2, 3, 4, 5];

  onPreview(): void {
    if (this.previewBox) {
      this.previewBox.nativeElement.textContent = this.feedback.comments;
    }
  }

  onSubmit(): void {
    if (!this.feedback.name?.trim() || !this.feedback.email?.trim()) {
      this.statusMessage = 'Name and email are required.';
      this.isSuccess = false;
      return;
    }

    if (this.feedback.name.length > 100 || this.feedback.email.length > 150) {
      this.statusMessage = 'Name (max 100) or email (max 150) exceeds maximum length.';
      this.isSuccess = false;
      return;
    }

    if (!this.feedback.comments?.trim()) {
      this.statusMessage = 'Please enter your feedback comments.';
      this.isSuccess = false;
      return;
    }

    if (this.feedback.comments.length > this.maxCommentLength) {
      this.statusMessage = `Comments cannot exceed ${this.maxCommentLength} characters.`;
      this.isSuccess = false;
      return;
    }

    this.isSubmitting = true;
    this.statusMessage = '';

    // Simulate async submission
    setTimeout(() => {
      this.isSubmitting = false;
      this.isSuccess = true;
      this.statusMessage = 'Thank you! Your feedback has been submitted successfully.';
      this.resetForm();
    }, 600);
  }

  private resetForm(): void {
    this.feedback = {
      name: '',
      email: '',
      feedbackType: 'general',
      rating: 5,
      comments: ''
    };
  }
}
