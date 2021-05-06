import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CollapsedService {
  liveCollapseArr: boolean[] = [];
  scoreboardCollapse: boolean = false;

  constructor() { }

  /*for collapse functionality*/
  initCollapse(arr: boolean[],size: number) {
    arr.length = size;
    for (let i = 0; i < size; i++) {
      arr[i] = false;
    }
  }
  collapseCloseOthers (arr: boolean[],idx: number) {
    //console.log('idx '+idx);
    for (let i=0 ; i<arr.length;i++){
      //console.log('loop num '+ i);

      if (i !== idx){
        arr[i] = false;
      }
      //console.log('loop actions '+ arr[i]);
    }
  }
  getLiveArr(i){
    return this.liveCollapseArr[i];
  }
  reverseLiveArr(i){
    this.liveCollapseArr[i] = !this.liveCollapseArr[i];
    //console.log('i '+ i+ ' arr[i] '+this.liveCollapseArr[i]);
  }
  closeScoreBoard(){
    this.scoreboardCollapse = false;
  }
}
