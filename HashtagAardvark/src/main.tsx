import * as React from 'react';
import  * as ReactDOM from 'react-dom';

import bind from 'bind-decorator';

import { AvGadget, AvTransform, AvPanel, AvGrabbable, HighlightType, GrabResponse, AvSphereHandle } from '@aardvarkxr/aardvark-react';
import { EndpointAddr, AvGrabEvent, endpointAddrToString } from '@aardvarkxr/aardvark-shared';


interface TestPanelState
{
	count: number;
	grabbableHighlight: HighlightType;
}

interface TestSettings
{
	count: number;
}

class TestPanel extends React.Component< {}, TestPanelState >
{
	private m_panelId?: EndpointAddr;

	constructor( props: any )
	{
		super( props );
		this.state = 
		{ 
			count: 0,
			grabbableHighlight: HighlightType.None,
		};

		AvGadget.instance().registerForSettings( this.onSettingsReceived );
	}

	@bind public incrementCount()
	{
		this.setState( { count: this.state.count + 1 } );

		let newSettings: TestSettings = { count: this.state.count + 1 };
		AvGadget.instance().saveSettings( newSettings );
	}

	@bind public onHighlightGrabbable( highlight: HighlightType )
	{
		this.setState( { grabbableHighlight: highlight } );
	}

	@bind public onGrabRequest( grabRequest: AvGrabEvent ): Promise< GrabResponse >
	{
		// this is totally unnecessary, but a good test of the plumbing.
		let response: GrabResponse =
		{
			allowed: true,
		};
		return Promise.resolve( response );
	}

	@bind public onSettingsReceived( settings: TestSettings )
	{
		if( settings )
		{
			this.setState( { count: settings.count } );
		}
	}

	public render()
	{
		let sDivClasses:string;
		let scale = 0.2;
		switch( this.state.grabbableHighlight )
		{
			default:
			case HighlightType.None:
				sDivClasses = "FullPage NoGrabHighlight";
				break;

			case HighlightType.InRange:
				sDivClasses = "FullPage InRangeHighlight";
				break;

			case HighlightType.Grabbed:
				sDivClasses = "FullPage GrabbedHighlight";
				break;

			case HighlightType.InHookRange:
				sDivClasses = "FullPage GrabbedHighlight";
				break;
		
		}

		return (
			<div className={ sDivClasses } >
				<div>
					<AvGrabbable updateHighlight={ this.onHighlightGrabbable }
						onGrabRequest={ this.onGrabRequest }
						dropOnHooks={ true }>
						<AvSphereHandle radius={0.1} />
						
						<AvTransform uniformScale={ scale }>
							<AvPanel interactive={true}
								onIdAssigned={ (id: EndpointAddr) => { this.m_panelId = id } }/>
						</AvTransform>
					</AvGrabbable>
				</div>
				<div className="Label">Count: { this.state.count }</div>
				<div className="Button" onMouseDown={ this.incrementCount }>
					Click Me!
					</div> 

				{ this.m_panelId && 
					<div>
						My ID is { endpointAddrToString( this.m_panelId as EndpointAddr ) }
					</div>
				}
			</div>
		)
	}
}

ReactDOM.render( <TestPanel/>, document.getElementById( "root" ) );
