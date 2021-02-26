import {StringLiteralLike} from 'typescript';
import {Api} from './api';

export class TaskViewModel {

  constructor(name: string, description: string, startTask: string, periodicity: string, filterText: string, apiId: number) {
    this.name = name;
    this.description = description;
    this.startTask = startTask;
    this.periodicity = periodicity;
    this.filterText = filterText;
    this.apiId = apiId;
  }

  taskId = -1;
  name: string;
  description: string;
  startTask: string;
  periodicity: string;
  filterText: string;
  apiId: number;
}
