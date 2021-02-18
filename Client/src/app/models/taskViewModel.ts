import { StringLiteralLike } from "typescript";
import { Api } from "./api";

export class TaskViewModel{

    constructor(name: string, description: string,startTask: string, periodicityMin: number, filterText: string, apiId: number){
        this.name=name;
        this.description=description;
        this.startTask=startTask;
        this.periodicityMin=periodicityMin;
        this.filterText=filterText;
        this.apiId=apiId;
    }

    taskId: number = -1;
    name: string;
    description: string;
    startTask: string;
    periodicityMin: number;
    filterText: string;
    apiId: number;
}