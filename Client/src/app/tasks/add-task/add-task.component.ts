import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import {Api} from 'src/app/models/api';
import {TaskViewModel} from 'src/app/models/taskViewModel';
import {TaskService} from 'src/app/services/task.service';
import {DatePipe} from '@angular/common';
import {CronGenComponent, CronOptions} from 'ngx-cron-editor';

@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.css']
})
export class AddTaskComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              private dialogSource: MatDialogRef<AddTaskComponent>, private taskService: TaskService,
              public datepipe: DatePipe) {
  }

  formAdd = new FormGroup({
    name: new FormControl(null, [Validators.required]),
    description: new FormControl(null, [Validators.required]),
    periodicityMin: new FormControl(null, [Validators.required]),
    apiId: new FormControl(null, [Validators.required]),
    filterText: new FormControl(null, [Validators.required]),
    startDate: new FormControl(this.datepipe.transform(new Date, 'yyyy-MM-dd'), [Validators.required]),
    startTime: new FormControl(null, [Validators.required])
  });

  apiList: any;
  selectedApi = '';
  date = new Date();

  public cronOptions: CronOptions = {
    formInputClass: 'form-control cron-editor-input',
    formSelectClass: 'form-control cron-editor-select',
    formRadioClass: 'cron-editor-radio',
    formCheckboxClass: 'cron-editor-checkbox',
    defaultTime: '',
    hideMinutesTab: false,
    hideHourlyTab: false,
    hideDailyTab: false,
    hideWeeklyTab: false,
    hideMonthlyTab: false,
    hideYearlyTab: true,
    hideAdvancedTab: true,
    hideSpecificWeekDayTab: false,
    hideSpecificMonthWeekTab: false,
    use24HourTime: true,
    hideSeconds: false,
    cronFlavor: 'quartz'
  };

  formStr = '* * * * * *';

  ngOnInit(): void {
    this.taskService.getListApi().subscribe((result: Api[]) => {
      this.apiList = result;
    }, error => {
      alert(error.message);
    });

    this.formAdd.controls.apiId.valueChanges.subscribe(value => {
      this.formAdd.controls.filterText.setValue('');
    });
  }

  add() {
    this.taskService.addTask(new TaskViewModel(this.formAdd.value.name, this.formAdd.value.description,
      `${this.formAdd.value.startDate} ${this.formAdd.value.startTime}`, this.formAdd.value.periodicityMin,
      this.formAdd.value.filterText, this.formAdd.value.apiId)).subscribe((res: any) => {
      this.dialogSource.close();
    }, error => {
      if (error.status === 400) {
        alert('Отправлены неверные данные');
      }
      if (error.status === 500) {
        alert('Возникла ошибка на сервере');
      }
    });
  }
}
