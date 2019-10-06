import { Injectable } from '@angular/core';
import {BaseService} from './base.service';
import {Photo} from '../_models/photo';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PhotoService extends BaseService<Photo> {

  constructor(http: HttpClient) {
    super(http);
  }
  setPhotoToMain(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }
  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }
}
