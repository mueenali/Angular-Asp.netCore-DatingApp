import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {HttpClient, HttpParams} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {PaginatedResult} from '../_models/pagination';
import {map} from 'rxjs/operators';
import {User} from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export abstract class BaseService<T> {

  baseUrl: string = environment.apiUrl;
  protected constructor(public http: HttpClient) { }

  getAll(route: string, page, pageSize): Observable<PaginatedResult<T[]>> {
    const paginatedResult: PaginatedResult<T[]> = new PaginatedResult<T[]>();
    let httpParams = new HttpParams();
    if (page != null && pageSize != null) {
      httpParams = httpParams.append('pageNumber', page);
      httpParams = httpParams.append('pageSize', pageSize);
    }

    return this.http.get<T[]>(this.baseUrl + route, {observe: 'response', params: httpParams}).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  getOne(route: string, id: number): Observable<T> {
    return  this.http.get<T>(this.baseUrl + route + '/' + id);
  }
  add(route: string, model: T) {
    return this.http.post(this.baseUrl + route, model);
  }
  delete(route: string, id: number) {
    return this.http.delete(this.baseUrl + route + '/' + id);
  }
  update(route: string, id: number, model: T) {
    return this.http.put(this.baseUrl + route + '/' + id, model);
  }
}
