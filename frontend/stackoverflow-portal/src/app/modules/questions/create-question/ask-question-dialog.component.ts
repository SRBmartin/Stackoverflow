import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-ask-question-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ask-question-dialog.component.html',
  styleUrls: ['./ask-question-dialog.component.scss']
})
export class AskQuestionDialogComponent {
  @Output() closed = new EventEmitter<void>();
  @Output() submitted = new EventEmitter<{ title: string; description: string; file?: File }>();

  title: string = '';
  description: string = '';
  file: File | null = null;
  filePreviewUrl: string | null = null;
  loading: boolean = false;

  
  titleError: string | null = null;
  descriptionError: string | null = null;
  fileError: string | null = null;

  close() {
    if (this.loading) return;
    this.closed.emit();
  }

  submit() {
    this.titleError = null;
    this.descriptionError = null;
    this.fileError = null;

    let hasError = false;

    if (!this.title?.trim()) {
      this.titleError = 'This field is required';
      hasError = true;
    }

    if (!this.description?.trim()) {
      this.descriptionError = 'This field is required';
      hasError = true;
    }

    if (!this.file) {
      this.fileError = 'This field is required';
      hasError = true;
    }

    if (hasError) return;

    this.loading = true;
    this.submitted.emit({
      title: this.title.trim(),
      description: this.description.trim(),
      file: this.file ?? undefined
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files ? event.target.files[0] : event.dataTransfer?.files[0];
    if (file) {
      this.file = file;
      this.fileError = null; 
      const reader = new FileReader();
      reader.onload = () => this.filePreviewUrl = reader.result as string;
      reader.readAsDataURL(file);
    }
  }

  removeFile() {
    this.file = null;
    this.filePreviewUrl = null;
  }
}
