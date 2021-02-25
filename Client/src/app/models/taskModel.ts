export class TaskModel {

  constructor(name: string, description: string, startTask: string, periodicity: string, filterText: string, apiId: number,
              taskId: number, lastExecution: string, userId: number) {
    this.name = name;
    this.description = description;
    this.startTask = startTask;
    this.periodicity = periodicity;
    this.filterText = filterText;
    this.apiId = apiId;
    this.taskId = taskId;
    this.lastExecution = lastExecution;
    this.userId = userId;
  }

  taskId: number;
  lastExecution: string;
  userId: number;
  name: string;
  description: string;
  startTask: string;
  periodicity: string;
  filterText: string;
  apiId: number;
}
