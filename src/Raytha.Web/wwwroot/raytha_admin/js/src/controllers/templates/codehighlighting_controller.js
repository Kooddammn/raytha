import { Controller } from "stimulus"
import * as monaco from 'monaco-editor'

export default class extends Controller {
    static targets = ["editor", "textarea"]

    connect() {
        this.editor = monaco.editor.create(this.editorTarget,
            {
                value: this.textareaTarget.value,
                language: 'html',
                automaticLayout: true,
                scrollBeyondLastLine: false
            });

        this.boundEditorChangedEvent = this.updateEditorFieldValue.bind(this);
        this.editor.onDidChangeModelContent(this.boundEditorChangedEvent);
    }

    updateEditorFieldValue() {
        const content = this.editor.getValue();
        console.log(content);
        this.textareaTarget.value = content;
    }
}