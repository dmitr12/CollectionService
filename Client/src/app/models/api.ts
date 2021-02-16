export class Api{

    constructor(apiId: number, apiName: string, baseUrl: string, filterColumn: string){
        this.apiId = apiId;
        this.apiName = apiName;
        this.baseUrl = baseUrl;
        this.filterColumn = filterColumn;
    }

    apiId: number;
    apiName: string;
    baseUrl: string;
    filterColumn: string;
}