import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { UserService } from '../../../../core/services/user.service';
import { ToastServer } from '../../../../common/ui/toast/toast.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BasicInputComponent } from '../../../../components/shared/ui/input/input.component';
import { BasicButtonComponent } from '../../../../components/shared/ui/button/basic-button.component';

@Component({
  selector: 'app-general-information',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    BasicInputComponent,
    BasicButtonComponent
  ],
  templateUrl: './general-information.component.html',
  styleUrls: ['./general-information.component.scss']
})
export class GeneralInformationComponent implements OnInit {
  generalInfoForm!: FormGroup;
  photoPreview: string | null = null;
  selectedFile?: File;
  isDragOver = false;
  userId!: string;

  constructor(
    private readonly userService: UserService,
    private readonly toastService: ToastServer,
    private readonly fb: FormBuilder,
    private readonly cdr: ChangeDetectorRef
  ) {
    this.generalInfoForm = this.fb.group({
      name: ['', Validators.required],
      lastname: ['', Validators.required],
      state: ['', Validators.required],
      city: ['', Validators.required],
      address: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  private loadProfile(): void {
    this.userService.getProfile().subscribe({
      next: (res) => {
        this.userId = res.Id;

        this.generalInfoForm.patchValue({
          name: res.Name,
          lastname: res.Lastname,
          state: res.State,
          city: res.City,
          address: res.Address
        });

        if (this.userId) {
          this.userService.getPhotoBlob(this.userId).subscribe({
            next: (blob) => {
              this.photoPreview = URL.createObjectURL(blob);
            },
            error: (err) => {
              this.photoPreview = null;
            }
          });
        }
      },
      error: (err) => {
        this.toastService.showToast('Error loading profile', 'error');
      }
    });
  }

  onSave(): void {
  if (this.generalInfoForm.invalid) {
    this.toastService.showToast('All fields are required', 'error');
    return;
  }

  // Provera da li postoji slika
  if (!this.selectedFile && !this.photoPreview) {
    this.toastService.showToast('You must select a profile photo', 'error');
    return;
  }

  const updatedData: any = {
    Name: this.generalInfoForm.value.name,
    Lastname: this.generalInfoForm.value.lastname,
    State: this.generalInfoForm.value.state,
    City: this.generalInfoForm.value.city,
    Address: this.generalInfoForm.value.address
  };

  const finalize = () => {
    this.userService.updateUser(updatedData).subscribe({
      next: () => {
        this.toastService.showToast('Data and profile photo updated successfully', 'success');
        if (this.selectedFile) {
          this.userService.getPhotoBlob(this.userId).subscribe(blob => {
            this.photoPreview = URL.createObjectURL(blob);
          });
          this.selectedFile = undefined;
        }
      },
      error: (err) => {
        if (err.error?.code === 'User.NoChangesDetected') {
          this.toastService.showToast('Data and profile photo updated successfully', 'success');
          return;
        }
        this.toastService.showToast('Error while updating data', 'error');
      }
    });
  };

  if (this.selectedFile) {
    this.userService.uploadPhoto(this.selectedFile).subscribe({
      next: () => {
        finalize();
      },
      error: (err) => {
        this.toastService.showToast('Error while uploading photo', 'error');
      }
    });
  } else {
    finalize();
  }
}


  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
  }

  onFileDropped(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
    if (event.dataTransfer?.files.length) {
      this.selectedFile = event.dataTransfer.files[0];
      this.previewPhoto();
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
      this.previewPhoto();
    }
  }

  previewPhoto() {
    if (!this.selectedFile) return;
    const reader = new FileReader();
    reader.onload = () => (this.photoPreview = reader.result as string);
    reader.readAsDataURL(this.selectedFile);
  }

  removePhoto() {
    this.selectedFile = undefined;
    this.photoPreview = null;
  }
}
