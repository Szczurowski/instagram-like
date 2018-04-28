import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from "react-router-dom";
import 'isomorphic-fetch';
import { Result } from "../types/common";
import { Photo } from "../types/photo"

interface PhotoState {
    content: Photo[];
    isLoading: boolean;
}

export class Home extends React.Component<RouteComponentProps<{}>, PhotoState> {
    constructor(props: any) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.state = { content: [], isLoading: true }
    }

    componentDidMount() {
        fetch('api/photo')
            .then(response => response.json() as Promise<Result<Photo[]>>)
            .then(({ content }) => {
                console.log(content);
                this.setState({ content, isLoading: false });
            });
    }

    render() {
        const { content } = this.state;

        return <div>
            <h1>Upload an image for analysis</h1>
            
            <div>
                <input type="file" onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.handleChange(e.target.files)} />
            </div>
            <div>
                <table className="table">
                    <thead>
                    <tr>
                        <th>Id</th>
                        <th>Link</th>
                    </tr>
                    </thead>
                    <tbody>
                    {content.map(photo => 
                        <tr>
                            <td>{photo.id}</td>
                            <td><Link to={`/photoDetails/${photo.id}`}>{photo.name}</Link></td>
                        </tr>
                    )}
                    </tbody>
                </table>
            </div>
        </div>;
    }

    private handleChange(selectorFiles: FileList | null) {
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
