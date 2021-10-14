import smcat from 'state-machine-cat';

export function renderToString (stateMachineString) {
    try {
        return smcat.render(stateMachineString
        ,
            {
                outputType: "svg",
            }
        );
    } catch (error) {
        console.log(error);
    }
    return "";
}