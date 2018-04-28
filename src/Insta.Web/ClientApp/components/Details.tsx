import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Result } from "../types/common";
import { PhotoDetailed, Caption } from "../types/photoDetailed";


interface PhotoDetailedState {
    content: PhotoDetailed;
    isLoading: boolean;
}

interface PhotoDetailedParams {
    id: number;
}

export class Details extends React.Component<RouteComponentProps<PhotoDetailedParams>, PhotoDetailedState> {
    constructor(props: RouteComponentProps<PhotoDetailedParams>) {
        super(props);

        this.state = {
            isLoading: true,
            content: {
                id: 0,
                name: '',
                originalLocation: '',
                processingAnalysisResult: {
                    description: {
                        captions: [],
                        tags: []
                    }
                }
            }
        };
    }

    componentDidMount() {
        const { match: { params } } = this.props;

        fetch(`api/photo/${params.id}`)
            .then(response => response.json() as Promise<Result<PhotoDetailed>>)
            .then(({ content }) => {
                this.setState({ content, isLoading: false });
            });
    }

    render() {
        const { content: { name, originalLocation, processingAnalysisResult: { description } } } = this.state;
        const text = description.captions.length > 0 ? description.captions[0].text : 'No caption';

        return <div>
            <h2>Details</h2>
            <h3>Computer Vision Analysis Results of file <b>{name}</b></h3>

            <img src={originalLocation} className="img-thumbnail" alt={name} />

            <b>{text}</b>

        </div>;
    }
}
