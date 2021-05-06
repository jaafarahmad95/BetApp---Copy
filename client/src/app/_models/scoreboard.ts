export interface Message {
	messageType: number;
	content: string;
	timer: string;
	order: number;
	teamType: string;
}

export interface scoreboard {
	redCards1: number;
	redCards2: number;
	penalties1: number;
	penalties2: number;
	yellowCards1: number;
	yellowCards2: number;
	messages: Message[];
	score: string;
	teamName1: string;
	teamName2: string;
	shirtColor1: string;
	shortsColor1: string;
	shirtColor2: string;
	shortsColor2: string;
	firsthalf1teamscore: number;
	firsthalf2teamscore: number;
	seconedhalf1teamscore: number;
	seconedhalf2teamscore: number;
	firstEhalf1teamscore: number;
	seconedEhalf1teamscore: number;
	firstEhalf2teamscore: number;
	seconedEhalf2teamscore: number;
	period: string;
}
