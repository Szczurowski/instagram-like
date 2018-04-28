import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

export class Home extends React.Component<RouteComponentProps<{}>, {}> {
    constructor(props: any) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
    }

    render() {
        return <div>
            <h1>Hello, world!</h1>

            <div>
                <input type="file" onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.handleChange(e.target.files)} />
           </div>
        </div>;
    }

    private handleChange(selectorFiles: FileList | null) {
        console.log(selectorFiles);

        if (selectorFiles != null) {
            const file = selectorFiles[0];
            fetch('api/Photo',
                    {
                        body: file,
                        method: 'POST',
                        headers: {
                            'x-filename': file.name
                        }
                    })
                .then(response => {
                    if (!response.ok) {
                        console.log(response.statusText);
                    }
                    return response.json() as Promise<{}>;
                })
                .then(data => {
                    console.log('Data received: ', data);
                }).catch(error => {
                    console.log(error);
                });
        }
    }
}

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}