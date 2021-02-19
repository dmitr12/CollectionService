import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Api } from '../models/api';
import { TaskModel } from '../models/taskModel';
import { TaskViewModel } from '../models/taskViewModel';
import { UserTasksInfo} from '../models/userTasksInfo';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  constructor(private httpClient: HttpClient) { }

  getListApi(): Observable<Api[]>{
    return this.httpClient.get<Api[]>(`${environment.url}api/task/apies`);
  }

  getTasksByUserId(userId: number): Observable<TaskModel[]>{
    return this.httpClient.get<TaskModel[]>(`${environment.url}api/task/GetTasksByUserId/${userId}`);
  }

  getTaskById(taskId: number): Observable<TaskModel>{
    return this.httpClient.get<TaskModel>(`${environment.url}api/task/gettaskbyid/${taskId}`);
  }

  getStatistics(): Observable<UserTasksInfo[]>{
    return this.httpClient.get<UserTasksInfo[]>(`${environment.url}api/task/getstatistics`);
  }

  deleteTask(idTask: number){
    return this.httpClient.delete(`${environment.url}api/task/DeleteTask/${idTask}`);
  }

  addTask(taskModel: TaskViewModel){
    return this.httpClient.post(`${environment.url}api/task/addtask`, taskModel);
  }

  updateTask(taskModel: TaskViewModel){
    return this.httpClient.put(`${environment.url}api/task/updatetask`, taskModel);
  }
}
