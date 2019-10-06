import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export abstract class BaseService<T> {

  baseUrl: string = environment.apiUrl;
  protected constructor(public http: HttpClient) { }

  getAll(route: string): Observable<Set<T>> {
    return this.http.get<Set<T>>(this.baseUrl + route);
  }
  getOne(route: string, id: number): Observable<T> {
    return  this.http.get<T>(this.baseUrl + route + '/' + id);
  }
  add(route: string, model: any) {
    return this.http.post(this.baseUrl + route, model);
  }
  delete(route: string, id: number) {
    return this.http.delete(this.baseUrl + route + '/' + id);
  }
  update(route: string, id: number, model: any) {
    return this.http.put(this.baseUrl + route + '/' + id, model);
  }
}
