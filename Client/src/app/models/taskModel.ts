export class TaskModel{

    constructor(name: string, description: string,startTask: string, periodicityMin: number, filterText: string, apiId: number,
        taskId: number, lastExecution: string, userId: number, countExecutions: number){
        this.name=name;
        this.description=description;
        this.startTask=startTask;
        this.periodicityMin=periodicityMin;
        this.filterText=filterText;
        this.apiId=apiId;
        this.taskId=taskId;
        this.lastExecution=lastExecution;
        this.userId=userId;
        this.countExecutions=countExecutions;
    }

    taskId: number;
    lastExecution: string;
    userId: number;
    countExecutions: number;
    name: string;
    description: string;
    startTask: string;
    periodicityMin: number;
    filterText: string;
    apiId: number;
}