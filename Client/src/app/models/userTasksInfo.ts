export class UserTasksInfo{

    constructor(userId: number, userName: string, countCompletedTasks: number, countActiveTasks: number){
        this.userId=userId;
        this.userName=userName;
        this.countCompletedTasks=countCompletedTasks;
        this.countActiveTasks=countActiveTasks;
    }

    userId: number;
    userName: string;
    countCompletedTasks: number;
    countActiveTasks: number;
}