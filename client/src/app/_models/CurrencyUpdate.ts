export interface CurrencyUpdate {
  userId: string;
  CurrencyList: UpdateCurrencyDto[] ;
}
export interface UpdateCurrencyDto {
  CurrenceyId:number;
  value : number;
}
