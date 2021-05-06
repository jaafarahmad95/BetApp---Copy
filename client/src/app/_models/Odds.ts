export interface Odds {
  runnerName: string;
  marketName: string;
  side: string;
  odds: number;
  keepInPlay: boolean;
  status: string;
  liquidity: number;
  visibility: string;
}
