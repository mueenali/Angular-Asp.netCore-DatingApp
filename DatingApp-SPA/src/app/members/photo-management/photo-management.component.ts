import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Photo} from '../../_models/photo';
import { FileUploader } from 'ng2-file-upload';
import {environment} from '../../../environments/environment';
import {AuthService} from '../../_services/auth.service';
import {AlertifyService} from '../../_services/alertify.service';
import {PhotoService} from '../../_services/photo.service';
@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  @Input() photos: Photo[];
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  mainPhoto: Photo;
  constructor(private authService: AuthService, private alertifyService: AlertifyService, private photoService: PhotoService) { }

  ngOnInit() {
    this.uploaderInit();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  uploaderInit() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5 : true,
      allowedFileType: ['image'],
      removeAfterUpload : true,
      autoUpload: false,
      maxFileSize: 8 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };

        this.photos.push(res);
        this.alertifyService.success('Photos uploaded successfully');

        if (photo.isMain) {
          this.updateMainPhoto(photo);
        }
      }
    };
  }

  setPhotoToMain(photo: Photo) {
    this.photoService.setPhotoToMain(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      for (let i = 0; i < this.photos.length; i++) {
        if (this.photos[i].isMain === true) {
          this.mainPhoto = this.photos[i];
          break;
        }
      }
      this.mainPhoto.isMain = false;
      photo.isMain = true;

      this.updateMainPhoto(photo);
      this.alertifyService.success('Photo set to main successfully');

    }, error => {
      this.alertifyService.error(error);
    });
  }

  deletePhoto(id: number) {
    this.alertifyService.confirm('Are you sure you want to delete this photo?', () => {
      this.photoService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
        this.photos.splice(this.photos.findIndex(photo => photo.id === id), 1);
        this.alertifyService.success('Your photo has been deleted successfully');
      }, error => this.alertifyService.error(error));
    });
  }

  private updateMainPhoto(photo: Photo) {
    this.authService.updateCurrentUserMainPhoto(photo.url);
    this.authService.user.photoUrl = photo.url;
    localStorage.setItem('user', JSON.stringify(this.authService.user));
  }
}
